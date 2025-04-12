namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents data submitted via the contact form.
/// </summary>
public class ContactViewModel
{
    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}