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
}
