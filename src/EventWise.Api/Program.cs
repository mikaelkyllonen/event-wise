
using EventWise.Api;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("User", policy => policy.RequireRole("user"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var jwtOptions = builder.Configuration
            .GetSection("Authentication:Schemes:Bearer")
            .Get<EventWise.Api.JwtBearerOptions>()!;

        options.Authority = jwtOptions.ValidAuthority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.ValidIssuers[0],
            ValidateAudience = true,
            ValidAudiences = jwtOptions.ValidAudiences,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SigningKeys[0].Value))
        };

        //if (builder.Environment.IsDevelopment())
        //{
        //    var signingKey = builder.Configuration.GetValue<string>("Authentication:Schemes:Bearer:SigningKey");

        //    if (!string.IsNullOrEmpty(signingKey))
        //    {
        //        var signingKeyBytes = Convert.FromBase64String(signingKey);
        //        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes);
        //    }
        //}
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!").AllowAnonymous();
app.MapGet("/another-endpoint", () => "Hello!").RequireAuthorization("User");
app.MapGet("/a", () => "Hello!").RequireAuthorization("User");

app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            var authorizedEndpoints = context.DescriptionGroups
                .SelectMany(descriptionGroup => descriptionGroup.Items)
                .Where(item => item.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
                .Select(x => x.RelativePath)
                .ToHashSet();

            var operations = document.Paths
                .Where(path => authorizedEndpoints.Contains(path.Key.TrimStart('/')))
                .SelectMany(path => path.Value.Operations)
                .ToArray();

            foreach (var operation in operations)
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },


                        }
                    ] = Array.Empty<string>()
                });
            }
        }
    }
}