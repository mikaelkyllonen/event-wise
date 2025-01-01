using EventWise.Api.Events;

namespace EventWise.Api.UnitTests;

internal sealed class TestData
{
    internal static BaseEvent CreateEventWith(EventState eventState)
    {
        var @event = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            10,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        switch (eventState)
        {
            case EventState.Published:
                break;
            case EventState.InProgress:
                @event.Start();
                break;
            case EventState.Completed:
                @event.Start();
                @event.Complete();
                break;
            case EventState.Canceled:
                @event.Cancel();
                break;
            default:
                throw new ArgumentException("Unknown event state", nameof(eventState));
        }

        return @event;
    }
}