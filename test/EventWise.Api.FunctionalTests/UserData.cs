using System.Net.Http.Json;

using Bogus;

using EventWise.Api.Users;

namespace EventWise.Api.FunctionalTests;

public static class UserData
{
    public static Guid UserGuid { get; } = Guid.NewGuid();

    public static RegisterUserRequest RegisterUserRequest => new(UserGuid, "Test", "User", "test@localhost.com");
}

internal static class HttpHelper
{
    internal static async Task<Guid> CreateEvent(this HttpClient client, Guid hostId)
    {
        var faker = new Faker();
        var request = new CreateEventRequest(
            faker.Random.String(10),
            faker.Random.String(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            faker.Date.Future(yearsToGoForward: 1),
            faker.Date.Future(yearsToGoForward: 2)
            );

        var response = await client.PostAsJsonAsync("events", request);
        var content = await response.Content.ReadFromJsonAsync<CreateEventResponse>();

        return content!.Id;
    }
}