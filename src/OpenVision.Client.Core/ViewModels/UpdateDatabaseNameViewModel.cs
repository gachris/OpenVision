using System.ComponentModel.DataAnnotations;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to update a database name.
/// </summary>
public class UpdateDatabaseNameViewModel
{
    /// <summary>
    /// Gets or sets the ID of the database.
    /// </summary>
    [Display(Name = "Database id")]
    [Required(ErrorMessage = "Database id is required.")]
    public virtual Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Database Name is required.")]
    public virtual string? Name { get; set; }
}