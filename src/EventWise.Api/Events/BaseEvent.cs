using EventWise.Api.Common;

namespace EventWise.Api.Events;

public abstract class BaseEvent(
    string name,
    string description,
    string location,
    DateTime startTime,
    DateTime endTime)
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Location { get; private set; } = location;
    public DateTime StartTimeUtc { get; private set; } = startTime;
    public DateTime? EndTimeUtc { get; private set; } = endTime;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    protected static Result Validate(DateTime startTime, DateTime endTime)
    {
        if (startTime > endTime)
        {
            return Result.Failure(EventErrors.StartTimeAfterEndTime);
        }
        if (startTime < DateTime.UtcNow)
        {
            return Result.Failure(EventErrors.StartTimeInPast);
        }

        return Result.Success();
    }
}

public static class EventErrors
{
    public static readonly Error StartTimeAfterEndTime = new(
        "Event.StartTimeAfterEndTime",
        "Start time cannot be after end time");

    public static readonly Error StartTimeInPast = new(
        "Event.StartTimeInPast",
        "Start time cannot be in the past");

    public static readonly Error MaxParticipantsLessThanOne = new(
        "Event.MaxParticipantsLessThanOne",
        "Max participants cannot be less than 1");

    public static readonly Error MaxParticipantsGreaterThanMax = new(
        "Event.MaxParticipantsGreaterThanMax",
        $"Max participants cannot be greater than {UserEvent.MaxParticipantsForUserEvents}");
}

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
        DateTime endTimeUtc)
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
        DateTime endTimeUtc)
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