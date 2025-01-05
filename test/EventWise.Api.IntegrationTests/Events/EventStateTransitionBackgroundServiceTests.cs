using EventWise.Api.IntegrationTests.Infrastructure;

namespace EventWise.Api.IntegrationTests.Events;

public sealed class EventStateTransitionBackgroundServiceTests(WebAppFactory factory) : BaseIntegrationTests(factory)
{
    [Fact]
    public async Task Starts_published_events_when_start_time_reached()
    {

    }
}
