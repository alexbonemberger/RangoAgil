using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgilApi.DbContexts;
using RangoAgilApi.Domain;
using RangoAgilApi.Entities;

namespace RangoAgilApi.EndpointsHandlers;

public static class RangosHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
    ([FromQuery(Name = "name")] string? rangoNome,
    RangoDbContext rangoDbContext,
    IMapper mapper)
    {
        var rangosEntity = await rangoDbContext.Rangos
                                .Where(x => rangoNome == null || x.Name.ToLower().Contains(rangoNome.ToLower()))
                                .ToListAsync();
        if (rangosEntity.Count <= 0 || rangosEntity == null)
            return TypedResults.NoContent();
        else
            return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
    }

    public static async Task<Results<NotFound, Ok<RangoDTO>>> GetRangosByIdAsync (
    int id,
    RangoDbContext rangoDbContext,
    IMapper mapper,
    ILogger<RangoDTO> logger)
    {
        var rangoEntity = await rangoDbContext.Rangos
        .Where(x => x.Id == id).FirstOrDefaultAsync();

        logger.LogInformation("retronou: " + rangoEntity?.Name);// Mesmo que .Name ?? "");
        logger.LogInformation("retronou: " + rangoEntity?.Name ?? "vazio");

        if (rangoEntity == null)
            return TypedResults.NotFound();
        else
            return TypedResults.Ok(mapper.Map<RangoDTO>((rangoEntity)));
    }

    public static async Task<CreatedAtRoute<RangoDTO>> CreateRangoAsync (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreationDTO rangoForCreationDTO)
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreationDTO);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(rangoToReturn, "GetRango", new { id = rangoToReturn.Id});
    }

    public static async Task<Results<NotFound, Ok>> UpdateRangoAsync(
    int id,
    RangoDbContext rangoDbContext,
    [FromBody] RangoForCreationDTO rangoForCreationDTO,
    IMapper mapper)
    {
        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
        if (rangoEntity == null)
            return TypedResults.NotFound();

        mapper.Map(rangoForCreationDTO, rangoEntity);

        await rangoDbContext.SaveChangesAsync();
        
        return TypedResults.Ok();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync(
    int id,
    RangoDbContext rangoDbContext)
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
        if (rangosEntity == null)
            return TypedResults.NotFound();

        rangoDbContext.Rangos.Remove(rangosEntity);

        await rangoDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }

}