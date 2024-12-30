using System.Net;
using System.Net.Http.Json;

using EventWise.Api.FunctionalTests.Infrastructure;

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

    [Fact]
    public async Task CreateEvent_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createEventRequest = new CreateEventRequest(
            "Test Event",
            "Test Description",
            "Test Location",
            10,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(-1));

        // Act
        var response = await UserClient.PostAsJsonAsync("events", createEventRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}