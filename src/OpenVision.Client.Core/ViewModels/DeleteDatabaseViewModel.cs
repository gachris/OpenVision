using System.ComponentModel.DataAnnotations;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to delete a database.
/// </summary>
public class DeleteDatabaseViewModel
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

    /// <summary>
    /// Gets or sets the confirm name of the database.
    /// </summary>
    [Display(Name = "Type Database name to confirm deletion")]
    [Compare(nameof(Name), ErrorMessage = "Database name not matched.")]
    public virtual string? ConfirmName { get; set; }
}