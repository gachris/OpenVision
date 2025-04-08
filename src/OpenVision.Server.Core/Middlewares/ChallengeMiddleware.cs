using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using OpenVision.Shared;
using OpenVision.Shared.Responses;
using Error = OpenVision.Shared.Responses.Error;

namespace OpenVision.Server.Core.Middlewares;

/// <summary>
/// Middleware to handle challenge responses based on HTTP status codes.
/// </summary>
public class ChallengeMiddleware
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

    private readonly RequestDelegate _request;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ChallengeMiddleware"/> class.
    /// </summary>
    /// <param name="requestDelegate">The request delegate to invoke.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestDelegate"/> is null.</exception>
    public ChallengeMiddleware(RequestDelegate requestDelegate)
    {
        _request = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate), "Request delegate is required");
    }

    #region Methods

    /// <summary>
    /// Invokes the middleware to handle the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context), "HTTP context is required");
        }

        // Continue processing the request pipeline
        await _request(context);

        // Check the response status code and handle specific cases
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            // Return a more descriptive message for unauthorized requests.
            await WriteErrorAsync(context, ResultCode.Unauthorized, "Authentication is required. Please provide valid credentials and try again.");
        }
        else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            // Return a more descriptive message for forbidden requests.
            await WriteErrorAsync(context, ResultCode.Forbidden, "Access denied. You do not have permission to access this resource.");
        }
    }

    /// <summary>
    /// Writes an error response to the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <param name="resultCode">The result code indicating the type of error.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task WriteErrorAsync(HttpContext context, ResultCode resultCode, string message)
    {
        var errorCollection = new List<Error>();
        var error = new Error(resultCode, message);

        errorCollection.Add(error);

        var response = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);
        var result = JsonSerializer.Serialize(response, JsonSerializerOptions);

        context.Response.ContentType = ContentType;
        await context.Response.WriteAsync(result);
    }

    #endregion

}