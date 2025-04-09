using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides asynchronous methods to access and manipulate database data.
/// </summary>
public interface IDatabasesRepository : IGenericRepository<Database>
{
}
