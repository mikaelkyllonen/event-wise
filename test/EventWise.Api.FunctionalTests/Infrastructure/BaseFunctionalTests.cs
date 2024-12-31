using System.Net.Http.Headers;

using EventWise.Api.FunctionalTests.Infrastructure;

namespace EventWise.Api.FunctionalTests;

public abstract class BaseFunctionalTests(WebAppFactory factory) : IClassFixture<WebAppFactory>
{
    protected HttpClient Client { get; init; } = factory.CreateClient();
    protected HttpClient UserClient
    {
        get
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken());

            return client;
        }
    }
}