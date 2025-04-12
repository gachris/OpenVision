namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a database file.
/// </summary>
public record DatabaseFileDto
{
    /// <summary>
    /// Gets the filename of the downloaded file.
    /// </summary>
    public required virtual string Filename { get; init; }

    /// <summary>
    /// Gets the contents of the downloaded file.
    /// </summary>
    public required virtual byte[] FileContents { get; init; }

    /// <summary>
    /// Gets the content type of the downloaded file.
    /// </summary>
    public required virtual string ContentType { get; init; }
}