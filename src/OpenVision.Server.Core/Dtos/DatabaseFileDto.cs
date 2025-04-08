namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a database file.
/// </summary>
public class DatabaseFileDto
{
    /// <summary>
    /// Gets or sets the filename of the downloaded file.
    /// </summary>
    public required virtual string Filename { get; set; }

    /// <summary>
    /// Gets or sets the contents of the downloaded file.
    /// </summary>
    public required virtual byte[] FileContents { get; set; }

    /// <summary>
    /// Gets or sets the content type of the downloaded file.
    /// </summary>
    public required virtual string ContentType { get; set; }
}