using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// Represents a specification for filtering <see cref="ImageTarget"/> entities based on user-specific criteria.
/// This specification allows filtering by user identifier alone or by a combination of a target identifier and a user identifier.
/// </summary>
public class ImageTargetForUserSpecification : BaseSpecification<ImageTarget>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetForUserSpecification"/> class 
    /// with a filter that selects image targets associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    public ImageTargetForUserSpecification(string userId)
        : base(target => target.Database!.UserId == userId)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetForUserSpecification"/> class 
    /// with a filter that selects a specific image target associated with the specified user.
    /// </summary>
    /// <param name="targetId">The unique identifier of the image target.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    public ImageTargetForUserSpecification(Guid targetId, string userId)
        : base(target => target.Id == targetId && target.Database!.UserId == userId)
    {
    }
}