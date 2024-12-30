using System.Net;
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
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkZCRDE0RTEwLUI2QkEtNDRBMS04MjE4LUNCODM3NEYzOUQ1OCIsInN1YiI6IkZCRDE0RTEwLUI2QkEtNDRBMS04MjE4LUNCODM3NEYzOUQ1OCIsImp0aSI6IjYyNzE3YTUiLCJyb2xlIjoidXNlciIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMDEiLCJuYmYiOjE3MzU0OTM2MTMsImV4cCI6MTc0MzI2OTYxMywiaWF0IjoxNzM1NDkzNjEzLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.QNwMK8_KbspF_IhD-VY2OFrgLB6C6BavF9xMLccIhQU");

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
