namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response payload returned by the API.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public record ResponseDoc<TResult>
{
    /// <summary>
    /// Gets the actual response data.
    /// </summary>
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