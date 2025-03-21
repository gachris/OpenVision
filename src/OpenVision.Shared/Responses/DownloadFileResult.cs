using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the result of downloading a file.
/// </summary>
public class DownloadFileResult
{
    /// <summary>
    /// Gets the filename of the downloaded file.
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; }

    /// <summary>
    /// Gets the contents of the downloaded file.
    /// </summary>
    [JsonPropertyName("file_contents")]
    public byte[] FileContents { get; }

    /// <summary>
    /// Gets the content type of the downloaded file.
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; }

    /// <summary>
    /// Initializes a new instance of the DownloadFileResult class with the specified filename, file contents, and content type.
    /// </summary>
    /// <param name="filename">The filename of the downloaded file.</param>
    /// <param name="fileContents">The contents of the downloaded file.</param>
    /// <param name="contentType">The content type of the downloaded file.</param>
    public DownloadFileResult(string filename,
                              byte[] fileContents,
                              string contentType)
    {
        Filename = filename;
        FileContents = fileContents;
        ContentType = contentType;
    }
}