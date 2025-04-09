using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides asynchronous methods to access and manipulate API key data.
/// </summary>
public interface IApiKeysRepository : IGenericRepository<ApiKey>
{
}
