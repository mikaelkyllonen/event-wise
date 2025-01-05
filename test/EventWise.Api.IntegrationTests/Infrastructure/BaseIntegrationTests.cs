using Microsoft.Extensions.DependencyInjection;

namespace EventWise.Api.IntegrationTests.Infrastructure;

public class BaseIntegrationTests : IClassFixture<WebAppFactory>
{
    protected IServiceScope Scope { get; init; }
    protected ApplicationDbContext DbContext { get; init; }

    protected BaseIntegrationTests(WebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
