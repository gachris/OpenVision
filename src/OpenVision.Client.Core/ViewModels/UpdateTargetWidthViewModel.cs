using System.ComponentModel.DataAnnotations;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to update a target width.
/// </summary>
public class UpdateTargetWidthViewModel
{
    /// <summary>
    /// Gets or sets the ID of the target.
    /// </summary>
    [Display(Name = "Target id")]
    [Required(ErrorMessage = "Target id is required.")]
    public virtual Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the size of the target in the X dimension.
    /// </summary>
    [Display(Name = "Width")]
    [Required(ErrorMessage = "Target width is required.")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; set; }
}