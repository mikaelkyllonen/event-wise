using System.Net;
using System.Net.Http.Json;

using EventWise.Api.FunctionalTests.Infrastructure;

namespace EventWise.Api.FunctionalTests.Users;

public sealed class CreateUserTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task Cannot_create_user_with_duplicate_email()
    {
        // Arrange
        var userWithDuplicateEmail = UserData.RegisterUserRequest with
        {
            Id = Guid.NewGuid()
        };
        await UserClient.PostAsJsonAsync("users", UserData.RegisterUserRequest);

        // Act
        var response = await UserClient.PostAsJsonAsync("users", userWithDuplicateEmail);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Cannot_create_user_with_duplicate_id()
    {
        // Arrange
        var userWithDuplicateId = UserData.RegisterUserRequest with
        {
            Email = "unique@localhost.com"
        };
        await UserClient.PostAsJsonAsync("users", UserData.RegisterUserRequest);

        // Act
        var response = await UserClient.PostAsJsonAsync("users", userWithDuplicateId);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}