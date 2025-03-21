using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OpenVision.Client.Core.ViewModels;

/// <summary>
/// Represents a request to update a target metadata.
/// </summary>
public class UploadTargetMetadataViewModel
{
    /// <summary>
    /// Gets or sets the ID of the target.
    /// </summary>
    [Display(Name = "Target id")]
    [Required(ErrorMessage = "Target id is required.")]
    public virtual Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    [Display(Name = "Metadata Package")]
    [Required(ErrorMessage = "Target metadata file is required.")]
    public virtual IFormFile? Metadata { get; set; }
}