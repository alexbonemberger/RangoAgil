using RangoAgilApi.EndpointFilters;
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
        rangosEndPoints.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();
        rangosEndPointsId.MapPut("", RangosHandlers.UpdateRangoAsync)// Chain of Responsability, executa todos os filtros até enconterar um que retorne true
            .AddEndpointFilter(new FilterReadOnly(3))// Chain: se o valor for 4 executa esse teste e o proximo
            .AddEndpointFilter(new FilterReadOnly(4));
        rangosEndPointsId.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter(new FilterReadOnly(3))// Chain: se o valor for 3 executa apenas este
            .AddEndpointFilter(new FilterReadOnly(2))
            .AddEndpointFilter<LogNotFoundResponseFilter>();
        //.AddEndpointFilter<FilterReadOnly>();
    }// Resolução do Desafio os rangosEndPoints que recebem os filtros no AddEndpointFilter não os Maps como MapPost ou Put

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndPointIngredientes = endpointRouteBuilder.MapGroup("/rangos/{id:int}/ingredientes");

        rangosEndPointIngredientes.MapGet("", IngredientesHandlers.GetIngredientesAsync);
    }
}