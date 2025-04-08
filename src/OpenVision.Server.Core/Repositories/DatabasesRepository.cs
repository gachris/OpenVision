using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Repositories;

/// <summary>
/// Repository for accessing and manipulating database entities.
/// </summary>
public class DatabasesRepository : GenericRepository<Database>, IDatabasesRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesRepository"/> class using the provided application context.
    /// </summary>
    public DatabasesRepository(ApplicationDbContext applicationContext, ILogger<DatabasesRepository> logger)
        : base(applicationContext, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesRepository"/> class using a context factory.
    /// </summary>
    public DatabasesRepository(IDbContextFactory<ApplicationDbContext> applicationContextPool, ILogger<DatabasesRepository> logger)
        : base(applicationContextPool, logger)
    {
    }
}
