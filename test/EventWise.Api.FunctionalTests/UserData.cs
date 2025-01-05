using System.Net.Http.Headers;
using System.Net.Http.Json;

using Bogus;

using EventWise.Api.Users;

namespace EventWise.Api.FunctionalTests;

public static class UserData
{
    public static Guid DefaultUserGuid { get; } = Guid.NewGuid();

    public static RegisterUserRequest RegisterUserRequest => new(DefaultUserGuid, "Test", "User", "test@localhost.com");
}

internal static class HttpHelper
{
    internal static async Task<HttpResponseMessage> SendRequestAsUserAsync(
        this HttpClient client, 
        HttpMethod method,
        string url, 
        Guid userId, 
        object? content = null)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = content != null ? JsonContent.Create(content) : null,
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        };

        return await client.SendAsync(request);
    }

    internal static async Task<Guid> CreateEventWithHostAsync(this HttpClient client, Guid hostId, int? maxParticipants = default)
    {
        var faker = new Faker();
        var request = new CreateEventRequest(
            faker.Random.String(10),
            faker.Random.String(20),
            faker.Locale,
            maxParticipants ?? faker.Random.Int(2, 10),
            faker.Date.Soon(),
            faker.Date.Future());

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "events")
        {
            Content = JsonContent.Create(request),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(hostId)) }
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to create event: {await response.Content.ReadAsStringAsync()}");
        }

        var content = await response.Content.ReadFromJsonAsync<CreateEventResponse>();

        return content!.Id;
    }

    internal static async Task<Guid> CreateUserAsync(this HttpClient client)
    {
        var faker = new Faker();
        var userId = Guid.NewGuid();
        var request = new RegisterUserRequest(
            userId,
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.Email);

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "users")
        {
            Content = JsonContent.Create(request),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken(userId)) }
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to create user: {await response.Content.ReadAsStringAsync()}");
        }

        return userId;
    }
}