﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            AddBearerSecurityScheme(document);
            var authorizedEndpoints = GetAuthorizedEndpoints(context);
            AddBearerSecurityRequirementsToAuthorizedEndpoints(document, authorizedEndpoints);
        }
    }

    private static void AddBearerSecurityScheme(OpenApiDocument document)
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
    }

    private static HashSet<string?> GetAuthorizedEndpoints(OpenApiDocumentTransformerContext context)
    {
        return context.DescriptionGroups
            .SelectMany(descriptionGroup => descriptionGroup.Items)
            .Where(item => item.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
            .Select(x => x.RelativePath)
            .ToHashSet();
    }

    private static void AddBearerSecurityRequirementsToAuthorizedEndpoints(OpenApiDocument document, HashSet<string?> authorizedEndpoints)
    {
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
                        }
                    }
                ] = Array.Empty<string>()
            });
        }
    }
}