using EventWise.Api.Common;

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

    public static readonly Error MaxActiveEvents = new(
        "Event.MaxActiveEvents",
        $"Cannot have more than {MaxActiveEvents} active events");

    public static class Participation
    {
        public static readonly Error AlreadyParticipating = new(
            "Event.Participation.AlreadyParticipating",
            "Cannot join an event that you are already participating in");

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

        public static readonly Error NotParticipating = new(
            "Event.Participation.NotParticipating",
            "Cannot leave an event that you are not participating in");

        public static readonly Error CannotLeaveFinishedEvent = new(
            "Event.Participation.CannotLeaveFinished",
            "Cannot leave an event that has completed or been canceled");
    }

    public static class State
    {
        public static readonly Error CannotPublish = new(
            "Event.State.CannotPublish",
            "Event cannot be published in its current state");

        public static readonly Error CannotStart = new(
            "Event.State.CannotStart",
            "Event cannot be started in its current state");

        public static readonly Error CannotCancel = new(
            "Event.State.CannotCancel",
            "Event cannot be canceled in its current state");

        public static readonly Error CannotComplete = new(
            "Event.State.CannotComplete",
            "Event cannot be completed in its current state");
    }
}