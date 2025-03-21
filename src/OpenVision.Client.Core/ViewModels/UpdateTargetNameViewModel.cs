using System.ComponentModel.DataAnnotations;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to update a target name.
/// </summary>
public class UpdateTargetNameViewModel
{
    /// <summary>
    /// Gets or sets the ID of the target.
    /// </summary>
    [Display(Name = "Target id")]
    [Required(ErrorMessage = "Target id is required.")]
    public virtual Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Target Name is required.")]
    public virtual string? Name { get; set; }
}