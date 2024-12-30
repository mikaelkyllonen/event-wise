using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EventWise.Api.Extensions;

public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtOptions = builder.Configuration
                    .GetSection("Authentication:Schemes:Bearer")
                    .Get<JwtBearerOptions>() ?? throw new InvalidOperationException("Bearer authentication options are missing.");

                options.Authority = jwtOptions.ValidAuthority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = jwtOptions.ValidIssuers,
                    ValidateAudience = true,
                    ValidAudiences = jwtOptions.ValidAudiences,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                };

                if (builder.Environment.IsDevelopment() && jwtOptions.SigningKeys.Length > 0)
                {
                    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(jwtOptions.SigningKeys[0].Value));
                }
            });

        return builder;
    }
}