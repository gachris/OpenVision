using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

public interface IApiKeysRepository
{
    Task<IQueryable<ApiKey>> GetAsync();

    Task<bool> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
}