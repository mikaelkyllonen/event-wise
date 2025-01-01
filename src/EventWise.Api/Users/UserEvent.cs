using EventWise.Api.Common;
using EventWise.Api.Events;

namespace EventWise.Api.Users;

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
        DateTime? endTimeUtc)
    {
        var baseValidation = Validate(startTimeUtc, endTimeUtc);
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

    public override Result Participate(User user)
    {
        if (Participants.Count >= MaxParticipants)
        {
            return Result.Failure(EventErrors.Participation.EventFull);
        }

        return base.Participate(user);
    }
}