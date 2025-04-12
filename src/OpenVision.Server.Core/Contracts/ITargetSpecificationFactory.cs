using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Contracts;

public interface ITargetSpecificationFactory
{
    /// <summary>
    /// Creates a specification for image target based on the current authentication context.
    /// </summary>
    /// <returns>The specification to use when retrieving the target.</returns>
    ISpecification<ImageTarget> GetImageTargetSpecification();

    /// <summary>
    /// Creates a specification for image target based on the current authentication context.
    /// </summary>
    /// <param name="targetId">The target identifier.</param>
    /// <returns>The specification to use when retrieving the target.</returns>
    ISpecification<ImageTarget> GetImageTargetSpecification(Guid targetId);
}