using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

using Testcontainers.MsSql;

using TUnit.Core.Interfaces;

namespace EventWise.Api.FunctionalTests;

public sealed class WebAppFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .Build();

    public WebAppFactory()
    {
        InitializeAsync().Wait();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // https://github.com/dotnet/efcore/issues/35126
            ServiceDescriptor descriptor1 = services.SingleOrDefault(
               d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))!;

            if (descriptor1 is not null)
            {
                services.Remove(descriptor1);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString())
            );

            // Add test jwt authentication
            services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
                {
                    Issuer = "test",
                };

                options.TokenValidationParameters.ValidIssuer = "test";
                options.TokenValidationParameters.ValidAudience = "test";
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6011F845-CB2C-451E-A772-4150CD901706"));
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await InitializeTestUserAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    private async Task InitializeTestUserAsync()
    {
        var client = CreateClient();
        var token = JwtTokenGenerator.GenerateToken();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        //client.DefaultRequestHeaders.Add("Authorization", $"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkZCRDE0RTEwLUI2QkEtNDRBMS04MjE4LUNCODM3NEYzOUQ1OCIsInN1YiI6IkZCRDE0RTEwLUI2QkEtNDRBMS04MjE4LUNCODM3NEYzOUQ1OCIsImp0aSI6IjYyNzE3YTUiLCJyb2xlIjoidXNlciIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMDEiLCJuYmYiOjE3MzU0OTM2MTMsImV4cCI6MTc0MzI2OTYxMywiaWF0IjoxNzM1NDkzNjEzLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.QNwMK8_KbspF_IhD-VY2OFrgLB6C6BavF9xMLccIhQU");

        var res = await client.PostAsJsonAsync("users", new { Id = "FBD14E10-B6BA-44A1-8218-CB8374F39D58", FirstName = "Test", LastName = "User", Email = "test@localhost" });
        res.EnsureSuccessStatusCode();
    }
}

public static class JwtTokenGenerator
{
    public static string GenerateToken()
    {
        var token = new JwtSecurityToken(
            issuer: "test",
            audience: "test",
            claims: [new Claim(ClaimTypes.NameIdentifier, "FBD14E10-B6BA-44A1-8218-CB8374F39D58"), new Claim(ClaimTypes.Role, "user")],
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6011F845-CB2C-451E-A772-4150CD901706")),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
