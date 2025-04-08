using Microsoft.EntityFrameworkCore;
using OpenVision.EntityFramework.DbContexts;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring and enhancing the IServiceCollection with OpenVision DbContext services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the OpenVision ApplicationDbContext configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="optionsAction">An action to configure the DbContextOptionsBuilder.</param>
    /// <returns>The updated IServiceCollection instance.</returns>
    public static IServiceCollection AddOpenVisionDbContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<ApplicationDbContext>(optionsAction);
        return services;
    }

    /// <summary>
    /// Adds a pooled DbContext factory for the OpenVision ApplicationDbContext.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="optionsAction">An action to configure the DbContextOptionsBuilder.</param>
    /// <returns>The updated IServiceCollection instance.</returns>
    public static IServiceCollection AddOpenVisionPooledDbContextFactory(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>(optionsAction);
        return services;
    }
}
