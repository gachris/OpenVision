using Microsoft.AspNetCore.Mvc;

namespace OpenVision.Web.Core.Filters;

/// <summary>
/// Represents a query for browsing targets.
/// </summary>
public class TargetBrowserQuery : BrowserQuery, IBrowserQuery
{
    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
    /// </summary>
    [FromQuery(Name = "database_id")]
    public virtual Guid? DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the description of the target.
    /// </summary>
    [FromQuery(Name = "description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the target.
    /// </summary>
    [FromQuery(Name = "created")]
    public virtual DateTime? Created { get; set; }
}