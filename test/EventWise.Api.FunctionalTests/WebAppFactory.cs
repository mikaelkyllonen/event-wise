using System.Net.Http.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TUnit.Core.Interfaces;

namespace EventWise.Api.FunctionalTests;

public sealed class WebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // Add db context for testing
        });
    }
    public async Task InitializeAsync()
    {
        await InitializeTestUserAsync();
    }

    private async Task InitializeTestUserAsync()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("users", new { Id = Guid.CreateVersion7(), FirstName = "Test", LastName = "User", Email = "test@localhost" });
    }
}
