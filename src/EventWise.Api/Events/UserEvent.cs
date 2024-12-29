using EventWise.Api.Common;

namespace EventWise.Api.Events;

public sealed class UserEvent : BaseEvent
{
    public static readonly int MaxParticipantsForUserEvents = 10;

    public Guid HostId { get; private set; }
    public int MaxParticipants { get; private set; }

    private UserEvent(
        Guid hostId,
        string name,
        string description,
        string location,
        int maxParticipants,
        DateTime startTimeUtc,
        DateTime? endTimeUtc)
        : base(name, description, location, startTimeUtc, endTimeUtc)
    {
        HostId = hostId;
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
}