using EventWise.Api.Common;

namespace EventWise.Api.Events;

public sealed class EventParticipant
{
    public Guid EventId { get; private set; }
    public Guid ParticipantId { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    public BaseEvent Event { get; private set; } = default!;
    public User Participant { get; private set; } = default!;

    private EventParticipant()
    { }

    public static Result<EventParticipant> Create(Guid eventId, Guid participantId)
    {
        return new EventParticipant
        {
            EventId = eventId,
            ParticipantId = participantId,
            JoinedAtUtc = DateTime.UtcNow,
        };
    }
}

public sealed class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;

    public ICollection<EventParticipant> Participations { get; init; } = [];

    private User()
    { }

    public static Result<User> Create(Guid id, string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<User>(UserErrors.FirstNameRequired);
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<User>(UserErrors.LastNameRequired);
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<User>(UserErrors.EmailRequired);
        }

        return new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };
    }
}

public static class UserErrors
{
    public static readonly Error FirstNameRequired = new(
        "User.FirstNameRequired",
        "First name is required");

    public static readonly Error LastNameRequired = new(
        "User.LastNameRequired",
        "Last name is required");

    public static readonly Error EmailRequired = new(
        "User.EmailRequired",
        "Email is required");
}
