using EventWise.Api.Common;

namespace EventWise.Api.Events;

public abstract class BaseEvent(
    string name,
    string description,
    string location,
    DateTime startTime,
    DateTime? endTime)
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Location { get; private set; } = location;
    public EventState EventState { get; private set; } = EventState.Published;
    public DateTime StartTimeUtc { get; private set; } = startTime;
    public DateTime? EndTimeUtc { get; private set; } = endTime;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

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
}