using RangoAgilApi.EndpointsHandlers;

namespace RangoAgilApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndPoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosEndPointsId = rangosEndPoints.MapGroup("/{id:int}");

        rangosEndPoints.MapGet("", RangosHandlers.GetRangosAsync);
        rangosEndPointsId.MapGet("", RangosHandlers.GetRangosByIdAsync).WithName("GetRango");
        rangosEndPoints.MapPost("", RangosHandlers.CreateRangoAsync);
        rangosEndPointsId.MapPut("", RangosHandlers.UpdateRangoAsync);
        rangosEndPointsId.MapDelete("", RangosHandlers.DeleteRangoAsync);
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndPointIngredientes = endpointRouteBuilder.MapGroup("/rangos/{id:int}/ingredientes");

        rangosEndPointIngredientes.MapGet("", IngredientesHandlers.GetIngredientesAsync);
    }
}