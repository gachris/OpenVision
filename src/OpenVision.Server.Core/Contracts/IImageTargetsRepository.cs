using OpenVision.EntityFramework.Entities;

namespace OpenVision.Server.Core.Contracts;

public interface IImageTargetsRepository
{
    Task<IQueryable<ImageTarget>> GetAsync();

    Task<bool> CreateAsync(ImageTarget imageTarget, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(ImageTarget imageTarget, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(ImageTarget imageTarget, CancellationToken cancellationToken = default);
}