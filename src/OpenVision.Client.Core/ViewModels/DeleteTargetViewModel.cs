using System.ComponentModel.DataAnnotations;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to delete a target.
/// </summary>
public class DeleteTargetViewModel
{
    /// <summary>
    /// Gets or sets the ID of the target.
    /// </summary>
    [Display(Name = "Target id")]
    [Required(ErrorMessage = "Target id is required.")]
    public virtual Guid? TargetId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the database.
    /// </summary>
    [Display(Name = "Database id")]
    [Required(ErrorMessage = "Database id is required.")]
    public virtual Guid? DatabaseId { get; set; }
}