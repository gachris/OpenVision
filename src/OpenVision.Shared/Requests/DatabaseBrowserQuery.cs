namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a query for browsing databases.
/// </summary>
public record DatabaseBrowserQuery : BrowserQuery
{
    /// <summary>
    /// Gets or sets the description of the database.
    /// </summary>
    public virtual string? Name { get; init; }
}