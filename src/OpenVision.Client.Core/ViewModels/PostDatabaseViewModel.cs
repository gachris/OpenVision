using System.ComponentModel.DataAnnotations;
using OpenVision.Shared;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to create a new database.
/// </summary>
public class PostDatabaseViewModel
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Database Name is required.")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    [Display(Name = "Type")]
    [Required(ErrorMessage = "Database Type is required.")]
    public virtual DatabaseType? Type { get; set; }
}