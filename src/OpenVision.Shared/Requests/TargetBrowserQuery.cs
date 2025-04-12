using OpenVision.Shared.Contracts;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a query for browsing targets.
/// </summary>
public record TargetBrowserQuery : BrowserQuery, IBrowserQuery
{
    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
    /// </summary>
    public virtual Guid? DatabaseId { get; init; }
}