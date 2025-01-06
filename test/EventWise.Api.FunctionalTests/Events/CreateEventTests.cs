using System.Net;
using System.Net.Http.Json;

using Bogus;

using EventWise.Api.FunctionalTests.Infrastructure;
using EventWise.Api.Users;

namespace EventWise.Api.FunctionalTests.Events;

public sealed class CreateEventTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createEventRequest = new CreateEventRequest(
            "Test Event",
            "Test Description",
            "Test Location",
            10,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2));

        // Act
        var response = await UserClient.PostAsJsonAsync("events", createEventRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // TODO: Add validation errors
    [Fact]
    public async Task CreateEvent_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidStartTime = DateTime.UtcNow.AddDays(-1);
        var createEventRequest = new CreateEventRequest(
            "Test Event",
            "Test Description",
            "Test Location",
            10,
            invalidStartTime,
            default);

        // Act
        var response = await UserClient.PostAsJsonAsync("events", createEventRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Cannot_create_event_when_user_has_max_active_events()
    {
        // Arrange
        var faker = new Faker();
        var maxActiveEvents = MaxActiveEventsRule.MaxActiveEvents;
        var userId = await Client.CreateUserAsync();

        for (var i = 0; i < maxActiveEvents; ++i)
        {
            await Client.CreateEventWithHostAsync(userId);
        }

        var createEventRequest = new CreateEventRequest(
            faker.Random.String2(10),
            faker.Random.String2(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            faker.Date.Soon(),
            faker.Date.Future());

        // Act
        var response = await Client.SendRequestAsUserAsync(HttpMethod.Post, "events", userId, createEventRequest);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Event.MaxActiveEvents", responseContent); // TODO: Add type-safe validation
    }
}