using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides asynchronous methods to access and manipulate image target data.
/// </summary>
public interface IImageTargetsRepository : IGenericRepository<ImageTarget>
{
}
