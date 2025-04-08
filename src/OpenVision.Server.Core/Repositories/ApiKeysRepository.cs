using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Repositories;

/// <summary>
/// Repository for accessing and manipulating api key entities.
/// </summary>
public class ApiKeysRepository : GenericRepository<ApiKey>, IApiKeysRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeysRepository"/> class using the provided application context.
    /// </summary>
    public ApiKeysRepository(ApplicationDbContext applicationContext, ILogger<ApiKeysRepository> logger)
        : base(applicationContext, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeysRepository"/> class using a context factory.
    /// </summary>
    public ApiKeysRepository(IDbContextFactory<ApplicationDbContext> applicationContextPool, ILogger<ApiKeysRepository> logger)
        : base(applicationContextPool, logger)
    {
    }
}
