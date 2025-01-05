using Microsoft.Extensions.DependencyInjection;

namespace EventWise.Api.IntegrationTests.Infrastructure;

public class BaseIntegrationTests : IClassFixture<WebAppFactory>
{
    private readonly IServiceScope _scope;
    protected ApplicationDbContext DbContext { get; init; }

    protected BaseIntegrationTests(WebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
