﻿using EventWise.Api.Common;

namespace EventWise.Api.Events;

public abstract class BaseEvent(
    Guid hostId,
    string name,
    string description,
    string location,
    DateTime startTime,
    DateTime? endTime)
{
    private static readonly Dictionary<EventState, HashSet<EventState>> AllowedStateTransitions = new()
    {
        { EventState.Published, new() { EventState.InProgress, EventState.Canceled } },
        { EventState.InProgress, new() { EventState.Completed, EventState.Canceled } },
        { EventState.Completed, new() },
        { EventState.Canceled, new() }
    };

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

    protected static Result Validate(DateTime startTime, DateTime? endTime, DateTime? utcNow)
    {
        if (endTime.HasValue && startTime > endTime)
        {
            return Result.Failure(EventErrors.StartTimeAfterEndTime);
        }
        if (startTime < (utcNow ?? DateTime.UtcNow))
        {
            return Result.Failure(EventErrors.StartTimeInPast);
        }

        return Result.Success();
    }

    public virtual Result Participate(Guid userId)
    {
        if (Participants.Any(p => p.ParticipantId == userId))
        {
            return Result.Failure(EventErrors.Participation.AlreadyParticipating);
        }
        if (HostId == userId)
        {
            return Result.Failure(EventErrors.Participation.HostCannotParticipate);
        }
        if (EventState == EventState.Canceled)
        {
            return Result.Failure(EventErrors.Participation.EventCanceled);
        }
        if (EventState == EventState.Completed)
        {
            return Result.Failure(EventErrors.Participation.EventCompleted);
        }

        var participant = EventParticipant.Create(Id, userId).Value;
        _participants.Add(participant);

        return Result.Success();
    }

    public Result Leave(Guid userId)
    {
        var participant = Participants.FirstOrDefault(p => p.ParticipantId == userId);
        if (participant is null)
        {
            return Result.Failure(EventErrors.Participation.NotParticipating);
        }
        if (EventState == EventState.Completed || EventState == EventState.Canceled)
        {
            return Result.Failure(EventErrors.Participation.CannotLeaveFinishedEvent);
        }

        _participants.Remove(participant);

        return Result.Success();
    }

    public Result Start()
    {
        var result = ValidateStateTransitionTo(EventState.InProgress);
        if (result.IsFailure)
        {
            return result;
        }

        EventState = EventState.InProgress;

        return Result.Success();
    }

    public Result Cancel()
    {
        var result = ValidateStateTransitionTo(EventState.Canceled);
        if (result.IsFailure)
        {
            return result;
        }

        EventState = EventState.Canceled;

        return Result.Success();
    }

    public Result Complete()
    {
        var result = ValidateStateTransitionTo(EventState.Completed);
        if (result.IsFailure)
        {
            return result;
        }

        EventState = EventState.Completed;

        return Result.Success();
    }

    private Result ValidateStateTransitionTo(EventState newState)
    {
        if (!AllowedStateTransitions[EventState].Contains(newState))
        {
            return Result.Failure(newState switch
            {
                EventState.Published => EventErrors.State.CannotPublish,
                EventState.InProgress => EventErrors.State.CannotStart,
                EventState.Completed => EventErrors.State.CannotComplete,
                EventState.Canceled => EventErrors.State.CannotCancel,
                _ => throw new ArgumentException("Unknown event state", nameof(newState))
            });
        }

        return Result.Success();
    }
}