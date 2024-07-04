
using MiniValidation;
using RangoAgilApi.Domain;

namespace RangoAgilApi.EndpointFilters;

public class ValidateAnnotationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var rangoForCreationDTO = context.GetArgument<RangoForCreationDTO>(2);

        if (!MiniValidator.TryValidate(rangoForCreationDTO, out var valitationErrors))
        {
            return TypedResults.ValidationProblem(valitationErrors);
        }
        return await next(context);
    }
}
