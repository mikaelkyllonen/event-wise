using EventWise.Api.Events;
using EventWise.Api.Users;

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

        UpdateEventState(eventState, @event);

        return @event;
    }

    internal static BaseEvent CreateEventWithParticipant(User user, EventState state)
    {
        var @event = CreateEventWith(EventState.Published);
        @event.Participate(user);
        UpdateEventState(state, @event);

        return @event;
    }

    private static void UpdateEventState(EventState eventState, BaseEvent @event)
    {
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
    }
}