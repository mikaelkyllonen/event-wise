using System.Net;
using System.Net.Http.Headers;

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
        await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

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
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_leave_nonexistent_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        
        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, $"events/{Guid.NewGuid()}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });
     
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
}
