using EventWise.Api.Common;
using EventWise.Api.Users;

namespace EventWise.Api.Events;

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

    public static readonly Error EventFull = new(
        "Event.EventFull",
        "Event is full");

    public static readonly Error UserAlreadyParticipating = new(
        "Event.AlreadyParticipating",
        "User is already participating in the event");

    public static readonly Error EventCanceled = new(
        "Event.Canceled",
        "Cannot participate in a canceled event");

    public static readonly Error EventEnded = new(
        "Event.Ended",
        "Cannot participate in an event that has ended");

    public static readonly Error HostCannotParticipate = new(
        "Event.HostCannotParticipate",
        "Host cannot join as a participant in their own event");
}