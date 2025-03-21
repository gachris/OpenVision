namespace OpenVision.Client.Core.Models;

/// <summary>
/// Represents the pagination information for a list of items.
/// </summary>
public class Pager
{
    /// <summary>
    /// Gets or sets the total count of items to be paged.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the size of each page.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the name of the action method to be called for paging.
    /// </summary>
    public required string Action { get; set; }

    /// <summary>
    /// Gets or sets the search string for filtering the items.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether searching is enabled or not.
    /// </summary>
    public bool EnableSearch { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of pages to be displayed in the pager.
    /// </summary>
    public int MaxPages { get; set; } = 10;
}