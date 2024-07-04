using static System.Net.Mime.MediaTypeNames;

namespace RangoAgilApi.EndpointFilters;

public class FilterReadOnly : IEndpointFilter
{
    public readonly int _lockedId;
    public FilterReadOnly(int lockedId)
    {
        _lockedId = lockedId;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = context.GetArgument<int>(0);

        if (id == _lockedId)
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "item não modificavel"
            });

        var result = await next.Invoke(context);
        return result;
        
    }
}
