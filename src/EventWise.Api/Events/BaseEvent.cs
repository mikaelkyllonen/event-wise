namespace EventWise.Api.Events;

public abstract class BaseEvent
{
    //private BaseEvent(string name, string description, string location, DateTime startTime, DateTime endTime)
    //{
    //    Validate(name, description, location, startTime, endTime);
    //}
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Location { get; private set; } = location;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime EndTime { get; private set; } = endTime;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;


    protected static void Validate(string name, string description, string location, DateTime startTime, DateTime endTime)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));
        }
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));
        }
    }
}

public sealed class UserEvent : BaseEvent
{
    public Guid HostId { get; private set; }
    public int MaxParticipants { get; private set; }
    //public List<EventParticipant> Participants { get; private set; } = new();

    private UserEvent(
        Guid hostId,
        string name,
        string description,
        string location,
        int maxParticipants,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
        //: base(name, description, location, startTimeUtc, endTimeUtc)
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
        var baseValidation = ValidateBaseEvent(name, description, location, startTimeUtc, endTimeUtc);
        if (baseValidation.IsFailure)
            return Result.Failure<UserEvent>(baseValidation.Error);

        if (maxParticipants <= 0)
            return Result.Failure<UserEvent>("Max participants must be greater than zero.");

        return Result.Success(new UserEvent(
            hostId,
            name,
            description,
            location,
            maxParticipants,
            startTimeUtc,
            endTimeUtc));
    }
}
