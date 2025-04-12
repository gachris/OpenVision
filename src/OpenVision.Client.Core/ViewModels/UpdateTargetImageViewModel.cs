using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to update a target image.
/// </summary>
public class UpdateTargetImageViewModel
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
    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    [Display(Name = "File")]
    [Required(ErrorMessage = "Image is required. The image must be jpg or png.")]
    public virtual IFormFile? Image { get; set; }
}