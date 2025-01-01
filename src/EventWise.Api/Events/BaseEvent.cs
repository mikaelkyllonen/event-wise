using EventWise.Api.Common;
using EventWise.Api.Users;

namespace EventWise.Api.Events;

public abstract class BaseEvent(
    Guid hostId,
    string name,
    string description,
    string location,
    DateTime startTime,
    DateTime? endTime)
{
    protected readonly List<EventParticipant> _participants = [];

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid HostId { get; private set; } = hostId;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Location { get; private set; } = location;
    public EventState EventState { get; private set; } = EventState.Published;
    public DateTime StartTimeUtc { get; private set; } = startTime;
    public DateTime? EndTimeUtc { get; private set; } = endTime;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public IReadOnlyList<EventParticipant> Participants => _participants;

    protected static Result Validate(DateTime startTime, DateTime? endTime)
    {
        if (endTime.HasValue && startTime > endTime)
        {
            return Result.Failure(EventErrors.StartTimeAfterEndTime);
        }
        if (startTime < DateTime.UtcNow)
        {
            return Result.Failure(EventErrors.StartTimeInPast);
        }

        return Result.Success();
    }

    public virtual Result Participate(User user)
    {
        if (Participants.Any(p => p.ParticipantId == user.Id))
        {
            return Result.Failure(EventErrors.UserAlreadyParticipating);
        }

        if (EventState == EventState.Canceled)
        {
            return Result.Failure(EventErrors.EventCanceled);
        }

        if (EventState == EventState.Completed)
        {
            return Result.Failure(EventErrors.EventEnded);
        }

        if (HostId == user.Id)
        {
            return Result.Failure(EventErrors.HostCannotParticipate);
        }

        var participant = EventParticipant.Create(Id, user.Id).Value;
        _participants.Add(participant);

        return Result.Success();
    }

    public Result Cancel()
    {
        //if (EventState == EventState.Canceled)
        //{
        //    return Result.Failure(EventErrors.EventAlreadyCancelled);
        //}

        EventState = EventState.Canceled;

        return Result.Success();
    }

    public Result Complete()
    {
        //if (EventState == EventState.Completed)
        //{
        //    return Result.Failure(EventErrors.EventAlreadyCompleted);
        //}

        EventState = EventState.Completed;

        return Result.Success();
    }
}