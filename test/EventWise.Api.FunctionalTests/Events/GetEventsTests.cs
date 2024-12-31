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
        Assert.Empty(result!.Events);
    }
}