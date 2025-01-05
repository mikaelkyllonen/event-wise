using System.Net;
using System.Net.Http.Headers;

using EventWise.Api.FunctionalTests.Infrastructure;

namespace EventWise.Api.FunctionalTests.Events;

public sealed class JoinEventTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task User_can_join_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(UserData.DefaultUserGuid);

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_join_nonexistent_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{Guid.NewGuid()}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_join_full_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(userId, maxParticipants: 1);

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_join_event_twice()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(userId);
        await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task User_cannot_join_event_without_authentication()
    {
        // Act
        var response = await Client.PostAsync($"events/{Guid.NewGuid()}/participants", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Host_cannot_join_their_own_event()
    {
        // Arrange
        var userId = await Client.CreateUserAsync();
        var eventId = await Client.CreateEventWithHostAsync(userId);

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"events/{eventId}/participants")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // TODO: Add event finishing feature
    //[Fact]
    //public async Task User_cannot_join_finished_event()
    //{
    //    // Arrange
    //    var userId = await Client.CreateUserAsync();
    //    var eventId = await Client.CreateEventWithHostAsync(UserData.DefaultUserGuid);

    //    // Act
    //    var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"events/{eventId}/participants")
    //    {
    //        Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
    //    });

    //    // Assert
    //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //}
}