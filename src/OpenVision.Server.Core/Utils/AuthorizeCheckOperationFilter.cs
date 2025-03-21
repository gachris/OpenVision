using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using OpenVision.Server.Core.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpenVision.Server.Core.Utils;

/// <summary>
/// Operation filter for adding security requirements to Swagger operations based on Authorize attributes.
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    private readonly ApiConfiguration _apiConfiguration;

    /// <summary>
    /// Constructor for the AuthorizeCheckOperationFilter class.
    /// </summary>
    /// <param name="apiConfiguration">The API configuration containing security settings.</param>
    public AuthorizeCheckOperationFilter(ApiConfiguration apiConfiguration)
    {
        _apiConfiguration = apiConfiguration;
    }

    /// <summary>
    /// Applies security requirements to the Swagger operation based on Authorize attributes.
    /// </summary>
    /// <param name="operation">The Swagger operation being configured.</param>
    /// <param name="context">The context for the Swagger operation filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the method or its declaring type has the Authorize attribute
        var hasAuthorize = context.MethodInfo.DeclaringType != null &&
                            (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                             || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

        if (hasAuthorize)
        {
            // Add responses for 401 and 403 errors
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            // Define security requirements for the operation
            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }
                    ]
                    = [_apiConfiguration.Audience] // Use audience from API configuration
                }
            ];
        }
    }
}