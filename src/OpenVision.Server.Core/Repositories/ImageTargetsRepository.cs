using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Repositories;

/// <summary>
/// Repository for accessing and manipulating targets entities.
/// </summary>
public class ImageTargetsRepository : GenericRepository<ImageTarget>, IImageTargetsRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetsRepository"/> class using the provided application context.
    /// </summary>
    public ImageTargetsRepository(ApplicationDbContext applicationContext, ILogger<ImageTargetsRepository> logger)
        : base(applicationContext, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetsRepository"/> class using a context factory.
    /// </summary>
    public ImageTargetsRepository(IDbContextFactory<ApplicationDbContext> applicationContextPool, ILogger<ImageTargetsRepository> logger)
        : base(applicationContextPool, logger)
    {
    }
}
