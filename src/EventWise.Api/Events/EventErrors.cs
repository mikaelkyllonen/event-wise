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

    public static class Participation
    {
        public static readonly Error AlreadyParticipating = new(
            "Event.Participation.AlreadyParticipating",
            "User is already participating in this event");

        public static readonly Error EventFull = new(
            "Event.Participation.EventFull",
            "Cannot join a full event");

        public static readonly Error EventCanceled = new(
            "Event.Participation.EventCanceled",
            "Cannot join a canceled event");

        public static readonly Error EventCompleted = new(
            "Event.Participation.EventCompleted",
            "Cannot join a completed event");

        public static readonly Error HostCannotParticipate = new(
            "Event.Participation.HostCannotParticipate",
            "Host cannot join as a participant in their own event");
    }
}