using System.Net.Http.Json;

using EventWise.Api.FunctionalTests.Infrastructure;

namespace EventWise.Api.FunctionalTests.Events;
public sealed class GetEventsTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Fact]
    public async Task Returns_empty_list_when_no_events()
    {
        // Act
        var response = await UserClient.GetAsync("events");
     
        // Assert
        var result = await response.Content.ReadFromJsonAsync<GetEventsResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.Events);
    }

    [Fact]
    public async Task Does_not_return_user_events()
    {
        // Arrange
        var createEventRequest = new CreateEventRequest(
            "Test Event",
            "Test Description",
            "Test Location",
            10,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2));
        await UserClient.PostAsJsonAsync("events", createEventRequest);

        // Act
        var response = await UserClient.GetAsync("events");
        
        // Assert
        var result = await response.Content.ReadFromJsonAsync<GetEventsResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.Events);
    }
}