using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

public interface IDatabasesRepository
{
    Task<IQueryable<Database>> GetAsync();

    Task<bool> CreateAsync(Database database, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Database database, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Database database, CancellationToken cancellationToken = default);
}