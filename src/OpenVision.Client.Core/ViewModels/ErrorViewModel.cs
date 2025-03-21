namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents the model for error view.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request id of the error.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether to show the request id or not.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}