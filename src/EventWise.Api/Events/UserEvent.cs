using EventWise.Api.Common;
using EventWise.Api.Users;

namespace EventWise.Api.Events;

public sealed class UserEvent : BaseEvent
{
    public static readonly int MaxParticipantsForUserEvents = 10;

    public int MaxParticipants { get; private set; }

    public User Host { get; private set; } = default!;

    private UserEvent(
        Guid hostId,
        string name,
        string description,
        string location,
        int maxParticipants,
        DateTime startTimeUtc,
        DateTime? endTimeUtc)
        : base(hostId, name, description, location, startTimeUtc, endTimeUtc)
    {
        MaxParticipants = maxParticipants;
    }

    public static Result<UserEvent> Create(
        Guid hostId,
        string name,
        string description,
        string location,
        int maxParticipants,
        DateTime startTimeUtc,
        DateTime? endTimeUtc,
        DateTime? utcNow = default)
    {
        var baseValidation = Validate(startTimeUtc, endTimeUtc, utcNow);
        if (baseValidation.IsFailure)
        {
            return Result.Failure<UserEvent>(baseValidation.Error);
        }
        if (maxParticipants < 1)
        {
            return Result.Failure<UserEvent>(EventErrors.MaxParticipantsLessThanOne);
        }
        if (maxParticipants > MaxParticipantsForUserEvents)
        {
            return Result.Failure<UserEvent>(EventErrors.MaxParticipantsGreaterThanMax);
        }

        return new UserEvent(
        hostId,
        name,
        description,
        location,
        maxParticipants,
        startTimeUtc,
        endTimeUtc);
    }

    public override Result Participate(Guid userId)
    {
        if (Participants.Count >= MaxParticipants)
        {
            return Result.Failure(EventErrors.Participation.EventFull);
        }

        return base.Participate(userId);
    }
}