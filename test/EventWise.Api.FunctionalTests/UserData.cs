namespace EventWise.Api.FunctionalTests;

public static class UserData
{
    public static Guid UserGuid { get; } = Guid.NewGuid();

    public static RegisterUserRequest RegisterUserRequest => new(UserGuid, "Test", "User", "test@localhost.com");
}