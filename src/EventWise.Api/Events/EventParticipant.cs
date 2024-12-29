using EventWise.Api.Common;

namespace EventWise.Api.Events;

public sealed class EventParticipant
{
    public Guid EventId { get; private set; }
    public Guid ParticipantId { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    public BaseEvent Event { get; private set; } = default!;
    public User Participant { get; private set; } = default!;

    private EventParticipant()
    { }

    public static Result<EventParticipant> Create(Guid eventId, Guid participantId)
    {
        return new EventParticipant
        {
            EventId = eventId,
            ParticipantId = participantId,
            JoinedAtUtc = DateTime.UtcNow,
        };
    }
}