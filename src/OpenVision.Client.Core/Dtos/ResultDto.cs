namespace OpenVision.Client.Core.Dtos;

/// <summary>
/// Represents the result of an operation with an optional error message.
/// </summary>
/// <param name="Error">An optional error message. If null or empty, the operation succeeded.</param>
/// <param name="Exception">An optional exception that occurred during the operation.</param>
public record ResultDto(string? Error = null, Exception? Exception = null)
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(Error);
}
