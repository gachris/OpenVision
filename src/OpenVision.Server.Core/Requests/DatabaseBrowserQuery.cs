using Microsoft.AspNetCore.Mvc;

namespace OpenVision.Server.Core.Requests;

/// <summary>
/// Represents a query for browsing databases.
/// </summary>
public class DatabaseBrowserQuery : BrowserQuery
{
    /// <summary>
    /// Gets or sets the description of the database.
    /// </summary>
    [FromQuery(Name = "description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the database.
    /// </summary>
    [FromQuery(Name = "created")]
    public virtual DateTime? Created { get; set; }
}