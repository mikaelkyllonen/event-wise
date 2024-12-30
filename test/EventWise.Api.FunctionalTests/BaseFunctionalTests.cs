using TUnit.Core.Interfaces;

namespace EventWise.Api.FunctionalTests;

public abstract class BaseFunctionalTests(WebAppFactory factory)
{
    protected HttpClient HttpClient { get; init; } = factory.CreateClient();
}
