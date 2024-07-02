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

var rangosEndPoints = app.MapGroup("/rangos");
var rangosEndPointsId = rangosEndPoints.MapGroup("/{id:int}");
var rangosEndPointIngredientes = rangosEndPointsId.MapGroup("/ingredientes");

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

//rangosEndPoints.MapGet("/", async (RangoDbContext rangoDbContext)
//    => Results.Ok(await rangoDbContext.Rangos.ToListAsync()));

rangosEndPoints.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
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

rangosEndPointIngredientes.MapGet("", async (
    int id,
    RangoDbContext rangoDbContext,
    IMapper mapper) =>
{
    return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                               .Include(rango => rango.Ingredientes)
                               .FirstOrDefaultAsync(rango => rango.Id == id))?.Ingredientes);
});

rangosEndPointsId.MapGet("", async (
    int id,
    RangoDbContext rangoDbContext,
    IMapper mapper) =>
{
    return mapper.Map<RangoDTO>((await rangoDbContext.Rangos
    .Where(x => x.Id == id).FirstOrDefaultAsync()));
}).WithName("GetRango");

rangosEndPoints.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreationDTO rangoForCreationDTO) =>
    //LinkGenerator linkGenerator,
    //HttpContext httpContext) =>
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreationDTO);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(rangoToReturn, "GetRango", new { id = rangoToReturn.Id });

        // Referencia para alunos
        //var linkToReturn = linkGenerator.GetUriByName(
        //    httpContext,
        //    "GetRango",
        //    new { id = rangoToReturn.Id });

        //return TypedResults.Created(linkToReturn, rangoToReturn);
    }
);

rangosEndPointsId.MapPut("", async Task<Results<NotFound, Ok>> (
    int id,
    RangoDbContext rangoDbContext,
    [FromBody] RangoForCreationDTO rangoForCreationDTO,
    IMapper mapper) =>
    {
        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
        if (rangoEntity == null)
            return TypedResults.NotFound();

        mapper.Map(rangoForCreationDTO, rangoEntity);

        await rangoDbContext.SaveChangesAsync();
        
        return TypedResults.Ok();
    }
);

rangosEndPointsId.MapDelete("", async Task<Results<NotFound, NoContent>> (
    int id,
    RangoDbContext rangoDbContext) =>
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
        if (rangosEntity == null)
            return TypedResults.NotFound();

        rangoDbContext.Rangos.Remove(rangosEntity);

        await rangoDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
);

app.Run();