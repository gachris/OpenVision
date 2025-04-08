using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using OpenVision.Server.Core.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpenVision.Server.Core.Filters;

/// <summary>
/// Operation filter for adding security requirements to Swagger operations based on Authorize attributes.
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    #region Fields/Consts

    private readonly SwaggerConfiguration _swaggerConfiguration;

    #endregion

    /// <summary>
    /// Constructor for the AuthorizeCheckOperationFilter class.
    /// </summary>
    /// <param name="swaggerConfiguration">The swagger configuration containing security settings.</param>
    public AuthorizeCheckOperationFilter(SwaggerConfiguration swaggerConfiguration)
    {
        _swaggerConfiguration = swaggerConfiguration;
    }

    #region Methods

    /// <summary>
    /// Applies security requirements to the Swagger operation based on Authorize attributes.
    /// </summary>
    /// <param name="operation">The Swagger operation being configured.</param>
    /// <param name="context">The context for the Swagger operation filter.</param>
    public virtual void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType != null &&
                            (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                             || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

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
                    = [_swaggerConfiguration.Audience]
                }
            ];
        }
    }

    #endregion
}