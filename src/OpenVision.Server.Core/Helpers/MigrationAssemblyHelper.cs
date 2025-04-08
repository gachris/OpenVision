using OpenVision.Server.Core.Configuration;
using MySqlMigrationAssembly = OpenVision.EntityFramework.MySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = OpenVision.EntityFramework.PostgreSQL.Helpers.MigrationAssembly;
using SqlMigrationAssembly = OpenVision.EntityFramework.SqlServer.Helpers.MigrationAssembly;

namespace OpenVision.Server.Core.Helpers;

/// <summary>
/// Helper class to retrieve migration assembly names based on database provider type.
/// </summary>
public static class MigrationAssemblyHelper
{
    /// <summary>
    /// Retrieves the migration assembly name based on the specified database provider type.
    /// </summary>
    /// <param name="databaseProvider">The database provider configuration.</param>
    /// <returns>The name of the migration assembly.</returns>
    public static string? GetMigrationAssemblyByProvider(DatabaseConfiguration databaseProvider)
    {
        return databaseProvider.ProviderType switch
        {
            DatabaseProviderType.SqlServer => SqlMigrationAssembly.GetMigrationAssemblyName(),
            DatabaseProviderType.PostgreSQL => PostgreSQLMigrationAssembly.GetMigrationAssemblyName(),
            DatabaseProviderType.MySql => MySqlMigrationAssembly.GetMigrationAssemblyName(),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType), databaseProvider.ProviderType, "Unsupported database provider type.")
        };
    }
}
