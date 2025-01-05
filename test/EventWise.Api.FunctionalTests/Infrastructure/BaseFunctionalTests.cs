using System.Net.Http.Headers;

namespace EventWise.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTests : IClassFixture<WebAppFactory>
{
    protected HttpClient Client { get; init; }
    protected HttpClient UserClient { get; init; }

    protected BaseFunctionalTests(WebAppFactory factory)
    {
        Client = factory.CreateClient();

        UserClient = factory.CreateClient();
        UserClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken());
    }
}