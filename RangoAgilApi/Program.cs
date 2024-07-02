using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RangoAgilApi.DbContexts;
using RangoAgilApi.Domain;
using RangoAgilApi.Entities;
using System.Text.Json.Serialization;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RangoAgil API", Version = "v1" });
});

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RangoAgil API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.MapControllers();

app.MapGet("/", async (RangoDbContext rangoDbContext)
    => Results.Ok(await rangoDbContext.Rangos.ToListAsync()));

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
    ([FromQuery(Name = "name")] string? rangoNome,
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

app.MapPost("/rango", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreationDTO rangoForCreationDTO) =>
{
    var rangoEntity = mapper.Map<Rango>(rangoForCreationDTO);
    rangoDbContext.Add(rangoEntity);
    await rangoDbContext.SaveChangesAsync();

    var rangoToReturn = mapper.Map<RangoForCreationDTO>(rangoEntity);
    return TypedResults.Ok(rangoToReturn);
});

app.Run();