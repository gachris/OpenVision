namespace OpenVision.Client.Core.Helpers;

/// <summary>
/// A helper class for adding notifications to the HttpContext items dictionary.
/// </summary>
public class NotificationHelpers
{
    /// <summary>
    /// The key used to store the notification in the HttpContext items dictionary.
    /// </summary>
    public const string NotificationKey = "Vision.Notification";

    /// <summary>
    /// Represents an alert notification.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// The type of alert.
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// The message to display in the alert.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// The title to display in the alert.
        /// </summary>
        public required string Title { get; set; }
    }

    /// <summary>
    /// Specifies the possible types of alerts.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// Represents an informational alert.
        /// </summary>
        Info,

        /// <summary>
        /// Represents a success alert.
        /// </summary>
        Success,

        /// <summary>
        /// Represents a warning alert.
        /// </summary>
        Warning,

        /// <summary>
        /// Represents an error alert.
        /// </summary>
        Error
    }
}