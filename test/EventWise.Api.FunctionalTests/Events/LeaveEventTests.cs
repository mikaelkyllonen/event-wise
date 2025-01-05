using System.Net;

using EventWise.Api.FunctionalTests.Infrastructure;

namespace EventWise.Api.FunctionalTests.Events;

public sealed class LeaveEventTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task User_can_leave_event_they_are_joined_in()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(UserData.DefaultUserGuid);
        await Client.SendRequestAsUserAsync(HttpMethod.Post, $"events/{eventId}/participants", userId);

        // Act
        var response = await Client.SendRequestAsUserAsync(HttpMethod.Delete, $"events/{eventId}/participants", userId);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_leave_event_they_are_not_joined_in()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(UserData.DefaultUserGuid);

        // Act
        var response = await Client.SendRequestAsUserAsync(HttpMethod.Delete, $"events/{eventId}/participants", userId);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_leave_nonexistent_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();

        // Act
        var response = await Client.SendRequestAsUserAsync(HttpMethod.Delete, $"events/{Guid.NewGuid()}/participants", userId);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Unauthenticated_user_cannot_leave_event()
    {
        // Arrange
        var eventId = await Client.CreateEventWithHostAsync(UserData.DefaultUserGuid);
     
        // Act
        var response = await Client.DeleteAsync($"events/{eventId}/participants");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Host_cannot_leave_event_their_own_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(userId);
     
        // Act
        var response = await Client.SendRequestAsUserAsync(HttpMethod.Delete, $"events/{eventId}/participants", userId);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
