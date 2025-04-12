using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenVision.Shared.Responses;

namespace OpenVision.Shared.Extensions;

/// <summary>
/// Provides extension methods for deserializing HttpResponseMessage content.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Ensures the HTTP response was successful and deserializes the JSON content into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="options">Optional JSON serializer options. If not provided, default options are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>An object of type <typeparamref name="T"/> deserialized from the HTTP response content.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response was not successful.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the response content is null after deserialization.</exception>
    public static async Task<T> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Ensure the response status code indicates success.
        response.EnsureSuccessStatusCode();

        // Use provided options or fallback to default configuration.
        options ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var result = await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);
        
        return result == null ? throw new InvalidOperationException("The HTTP response content is null.") : result;
    }

    /// <summary>
    /// Reads the HTTP response content and deserializes it into a <see cref="ResponseMessage{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the response result.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="options">Optional JSON serializer options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ResponseMessage{T}"/> representing the API response.</returns>
    public static async Task<ResponseMessage<T>> ReadResponseMessageAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await response.ReadJsonAsync<ResponseMessage<T>>(options, cancellationToken);
    }

    /// <summary>
    /// Reads the HTTP response content and deserializes it into a <see cref="PagedResponse{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data contained in the paged response.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="options">Optional JSON serializer options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="PagedResponse{T}"/> representing the paged API response.</returns>
    public static async Task<PagedResponse<T>> ReadPagedResponseAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await response.ReadJsonAsync<PagedResponse<T>>(options, cancellationToken);
    }
}