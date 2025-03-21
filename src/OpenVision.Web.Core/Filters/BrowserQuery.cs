using Microsoft.AspNetCore.Mvc;

namespace OpenVision.Web.Core.Filters;

/// <summary>
/// Represents a base query for browsing items.
/// </summary>
public class BrowserQuery : PaginationFilter, IBrowserQuery
{
    /// <summary>
    /// Gets or sets the search text to filter items by.
    /// </summary>
    [FromQuery(Name = "search_text")]
    public virtual string? SearchText { get; set; }
}