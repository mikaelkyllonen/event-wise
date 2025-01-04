using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using EventWise.Api.Events;
using EventWise.Api.FunctionalTests.Infrastructure;
using EventWise.Api.Users;

using Microsoft.Extensions.DependencyInjection;

namespace EventWise.Api.FunctionalTests.Events;

public sealed class JoinEventTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task User_can_join_event()
    {
        // Arrange
        var userId = await UserClient.CreateUser();
        var eventId = await Client.CreateEvent(userId);

        // Create event
        //var createdResponse = await UserClient.PostAsJsonAsync("events", new CreateEventRequest("Test", "Test", "Location", 10, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));
        //var eventId = await createdResponse.Content.ReadFromJsonAsync<CreateEventResponse>();

        // Create user
        //var userGuid = Guid.NewGuid();
        //var res = await UserClient.PostAsJsonAsync("users", new RegisterUserRequest(userGuid, "Test", "User", "test2@localhost.com"));
        //Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userGuid));

        // Act
        var response = await UserClient.GetAsync($"events/{eventId}/participants");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task User_can_join_event2()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var host = User.Create(Guid.NewGuid(), "Test", "User", "test2@localhost.com").Value;
        var @event = UserEvent.Create(host.Id, "Test Event", "Test Description", "Test Location", 10, DateTime.UtcNow.AddDays(1), null).Value;

        await dbContext.Users.AddAsync(host);
        await dbContext.Events.AddAsync(@event);
        await dbContext.SaveChangesAsync();
        //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userGuid));

        // Act
        // Join event
        var response = await UserClient.GetAsync($"events/{@event.Id}/participants");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
