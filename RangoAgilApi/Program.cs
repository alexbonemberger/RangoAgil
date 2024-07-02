using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RangoAgilApi.DbContexts;
using RangoAgilApi.Domain;
using RangoAgilApi.Entities;
using System.Text.Json.Serialization;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// (add profile) De acordo com a classe Domain existe uma propriedade chamada current domain que é o dominio atual da api, dado esse domain ele vai pegar as dlls(assemblies) e ao rodar o build ele vai pesquisar quem herda de profile e atribui esse cara ao builder

var app = builder.Build();

app.MapGet("/", async (RangoDbContext rangoDbContext)
    => Results.Ok(await rangoDbContext.Rangos.ToListAsync()));

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
    ([FromQuery(Name = "name")]string ? rangoNome,
    RangoDbContext rangoDbContext,
    IMapper mapper) =>
    {
        var rangosEntity = await rangoDbContext.Rangos
                                .Where(x => rangoNome == null || x.Name.ToLower().Contains(rangoNome.ToLower()))
                                .ToListAsync();
        if (rangosEntity.Count <= 0 || rangosEntity == null)
            return TypedResults.NoContent();
        else
            return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
    });
    

app.MapGet("/rangos/{rangoId:int}/ingredientes", async (
    int rangoId,
    RangoDbContext rangoDbContext,
    IMapper mapper) =>
{
    return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                               .Include(rango => rango.Ingredientes)
                               .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

app.MapGet("/rangos/{id}", async (
    int id,
    RangoDbContext rangoDbContext,
    IMapper mapper) =>
    {
        return mapper.Map<RangoDTO>((await rangoDbContext.Rangos
        .Where(x => x.Id == id).FirstOrDefaultAsync()));
    });

app.Run();