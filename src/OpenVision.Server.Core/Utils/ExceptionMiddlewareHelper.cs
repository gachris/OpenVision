using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenVision.Shared;
using OpenVision.Shared.Exceptions;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Utils;

/// <summary>
/// Helper class to configure exception handling middleware for handling exceptions and returning appropriate JSON responses.
/// </summary>
public static class ExceptionMiddlewareHelper
{
    #region Fields/Consts

    private const string ContentType = "application/json";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    #endregion

    /// <summary>
    /// Configures global exception handling middleware to handle and log exceptions.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.ContentType = "application/json";
                    if (contextFeature.Error is HttpException exception)
                    {
                        // Handle known HTTP exceptions
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        var result = JsonSerializer.Serialize(exception.ErrorResponseMessage, JsonSerializerOptions);

                        await context.Response.WriteAsync(result);
                    }
                    else
                    {
                        // Handle other unexpected exceptions
                        var errorCollection = new List<Error>();

                        var error = new Error(ResultCode.InternalServerError, contextFeature.Error.Message);

                        errorCollection.Add(error);

                        var response = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);

                        var result = JsonSerializer.Serialize(response, JsonSerializerOptions);

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        context.Response.ContentType = ContentType;

                        await context.Response.WriteAsync(result);
                    }
                }
            });
        });
    }
}
