using Microsoft.FeatureManagement;

namespace EventWise.Api.Events;

public class LeaveEventFeatureFilter(IFeatureManager featureManager)
    : FeatureFilter(featureManager)
{
    public override string FeatureFlag => "LeaveEvent";
}