namespace OpenVision.Client.Core.Configuration;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Gets or sets the title of the page.
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the OpenVision server url.
    /// </summary>
    public required string OpenVisionServerUrl { get; set; }
}