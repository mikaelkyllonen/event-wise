namespace EventWise.Api.Users;

public sealed record RegisterUserRequest(Guid Id, string FirstName, string LastName, string Email);