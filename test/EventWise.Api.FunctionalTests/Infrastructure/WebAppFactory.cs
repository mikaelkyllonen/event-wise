﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Testcontainers.MsSql;

namespace EventWise.Api.FunctionalTests.Infrastructure;

public sealed class WebAppFactory() : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var jwtOptions = TestJwtConfiguration.CreateTestOptions();
        builder.ConfigureAppConfiguration((context, configBuilder) =>
            configBuilder.AddInMemoryCollection(TestJwtConfiguration.ToConfigurationDictionary(jwtOptions)));

        builder.ConfigureTestServices(services =>
        {
            // https://github.com/dotnet/efcore/issues/35126
            var descriptor = services.SingleOrDefault(
               d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))!;

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString())
            );

            services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
                {
                    Issuer = JwtTokenGenerator.Issuer,
                };

                options.TokenValidationParameters.ValidIssuer = JwtTokenGenerator.Issuer;
                options.TokenValidationParameters.ValidAudience = JwtTokenGenerator.Audience;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenGenerator.SigningKey));
            });

            services.AddLogging(builder => builder.AddDebug());
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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTokenGenerator.GenerateToken());

        var res = await client.PostAsJsonAsync("users", UserData.RegisterUserRequest);
        res.EnsureSuccessStatusCode();
    }
}