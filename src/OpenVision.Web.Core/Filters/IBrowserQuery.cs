namespace OpenVision.Web.Core.Filters;

/// <summary>
/// Represents a query for browsing items.
/// </summary>
public interface IBrowserQuery : IPaginationFilter
{
    /// <summary>
    /// Gets or sets the search text to filter items by.
    /// </summary>
    string? SearchText { get; set; }
}