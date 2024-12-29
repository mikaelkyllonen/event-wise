using EventWise.Api.Common;

namespace EventWise.Api.Events;

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