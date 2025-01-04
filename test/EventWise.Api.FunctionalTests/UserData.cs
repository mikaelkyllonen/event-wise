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
            faker.Date.Soon(),
            faker.Date.Soon(days: 2));

        client.DefaultRequestHeaders.Authorization = new("Bearer", JwtTokenGenerator.GenerateToken(hostId));
        var response = await client.PostAsJsonAsync("events", request);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to create event: {await response.Content.ReadAsStringAsync()}");
        }

        var content = await response.Content.ReadFromJsonAsync<CreateEventResponse>();

        return content!.Id;
    }

    internal static async Task<Guid> CreateUser(this HttpClient client)
    {
        var faker = new Faker();
        var userId = Guid.NewGuid();
        var request = new RegisterUserRequest(
            userId,
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.Email);

        var response = await client.PostAsJsonAsync("users", request);
        if (!response.IsSuccessStatusCode) 
        {
            throw new InvalidOperationException($"Failed to create user: {await response.Content.ReadAsStringAsync()}");
        }

        return userId;
    }
}