using AutoMapper;
using RangoAgilApi.DbContexts;
using RangoAgilApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RangoAgilApi.EndpointsHandlers;

public static class IngredientesHandlers
{
    public static async Task<Results<NotFound, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesAsync (
    int id,
    RangoDbContext rangoDbContext,
    IMapper mapper)
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
        if (rangosEntity == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                           .Include(rango => rango.Ingredientes)
                           .FirstOrDefaultAsync(rango => rango.Id == id))?.Ingredientes));
    }
}

