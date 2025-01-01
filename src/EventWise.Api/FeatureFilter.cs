using Microsoft.FeatureManagement;

namespace EventWise.Api;

public abstract class FeatureFilter(IFeatureManager featureManager) : IEndpointFilter
{
    private readonly IFeatureManager _featureManager = featureManager;

    public abstract string FeatureFlag { get; }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlag);

        return !isEnabled
            ? Results.NotFound()
            : await next(context);
    }
}