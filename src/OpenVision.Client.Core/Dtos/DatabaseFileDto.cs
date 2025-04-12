namespace OpenVision.Client.Core.Dtos;

/// <summary>
/// Represents the result of downloading a file.
/// </summary>
public record DatabaseFileDto
{
    /// <summary>
    /// Gets the filename of the downloaded file.
    /// </summary>
    public virtual required string Filename { get; init; }

    /// <summary>
    /// Gets the contents of the downloaded file.
    /// </summary>
    public virtual required byte[] FileContents { get; init; }

    /// <summary>
    /// Gets the content type of the downloaded file.
    /// </summary>
    public virtual required string ContentType { get; init; }
}