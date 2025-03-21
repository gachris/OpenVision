using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response payload returned by the API.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public class ResponseDoc<TResult>
{
    /// <summary>
    /// Gets the actual response data.
    /// </summary>
    [JsonPropertyName("result")]
    public virtual TResult Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseDoc{TResult}"/> class.
    /// </summary>
    /// <param name="result">The actual response data.</param>
    public ResponseDoc(TResult result)
    {
        Result = result;
    }
}