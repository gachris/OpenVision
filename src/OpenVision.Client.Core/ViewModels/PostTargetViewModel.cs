using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OpenVision.Shared;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to create a new target.
/// </summary>
public class PostTargetViewModel
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Target name is required.")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
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

    /// <summary>
    /// Gets or sets the size of the target in the X dimension.
    /// </summary>
    [Display(Name = "Width")]
    [Required(ErrorMessage = "Target width is required.")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    [Display(Name = "Type")]
    [Required(ErrorMessage = "Target type is required.")]
    public virtual TargetType? Type { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    [Display(Name = "Metadata Package")]
    public virtual IFormFile? Metadata { get; set; }
}