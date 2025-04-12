namespace OpenVision.Client.Core.Dtos;

/// <summary>
/// Represents the result of downloading a file.
/// </summary>
public class DatabaseFileDto
{
    /// <summary>
    /// Gets the filename of the downloaded file.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// Gets the contents of the downloaded file.
    /// </summary>
    public byte[] FileContents { get; }

    /// <summary>
    /// Gets the content type of the downloaded file.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Initializes a new instance of the DownloadFileResult class with the specified filename, file contents, and content type.
    /// </summary>
    /// <param name="filename">The filename of the downloaded file.</param>
    /// <param name="fileContents">The contents of the downloaded file.</param>
    /// <param name="contentType">The content type of the downloaded file.</param>
    public DatabaseFileDto(
        string filename,
        byte[] fileContents,
        string contentType)
    {
        Filename = filename;
        FileContents = fileContents;
        ContentType = contentType;
    }
}