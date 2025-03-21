namespace OpenVision.Web.Core.Filters;

/// <summary>
/// Represents a filter for pagination.
/// </summary>
public interface IPaginationFilter
{
    /// <summary>
    /// Gets or sets the number of items to include in a page.
    /// </summary>
    int Size { get; set; }

    /// <summary>
    /// Gets or sets the number of the page to retrieve.
    /// </summary>
    int Page { get; set; }
}