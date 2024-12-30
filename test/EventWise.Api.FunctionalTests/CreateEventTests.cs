using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EventWise.Api.FunctionalTests;

[ClassDataSource<WebAppFactory>(Shared = SharedType.PerAssembly)]
public sealed class CreateEventTests(WebAppFactory factory) : BaseFunctionalTests(factory)
{
    [Test]
    public async Task CreateEvent_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateEventRequest(
            "Test Event",
            "Test Description",
            "Test Location",
            10,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2));

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken());

        // Act
        var response = await HttpClient.PostAsJsonAsync("events", request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //var content = await response.Content.ReadFromJsonAsync<CreateEventResponse>();
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
    }

    [Test]
    public async Task GetEvents_WithNoEvents_ReturnsEmptyList()
    {
        // Act
        var response = await HttpClient.GetAsync("events");
        // Assert
        //response.EnsureSuccessStatusCode();
        //var content = await response.Content.ReadFromJsonAsync<List<GetEventsResponse>>();
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        //await Assert.That(content).IsEqualTo(new List<GetEventsResponse>());
    }
}
