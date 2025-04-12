namespace OpenVision.Client.Core.Configuration;

/// <summary>
/// Represents the open vision server options.
/// </summary>
public class OpenVisionApiOptions
{
    /// <summary>
    /// Gets or sets the URL of the OpenVision server url.
    /// </summary>
    public required string BaseUri { get; set; }
}