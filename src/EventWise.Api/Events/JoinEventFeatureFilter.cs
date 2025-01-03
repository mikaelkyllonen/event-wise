using Microsoft.FeatureManagement;

namespace EventWise.Api.Events;

public class JoinEventFeatureFilter(IFeatureManager featureManager)
    : FeatureFilter(featureManager)
{
    public override string FeatureFlag => "JoinEvent";
}