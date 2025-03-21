using System.Reflection;
using MySqlMigrationAssembly = OpenVision.Server.EntityFramework.MySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = OpenVision.Server.EntityFramework.PostgreSQL.Helpers.MigrationAssembly;
using SqlMigrationAssembly = OpenVision.Server.EntityFramework.SqlServer.Helpers.MigrationAssembly;

namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Configuration class to retrieve migration assembly names based on database provider type.
/// </summary>
public static class MigrationAssemblyConfiguration
{
    /// <summary>
    /// Retrieves the migration assembly name based on the specified database provider type.
    /// </summary>
    /// <param name="databaseProviderType">The database provider type.</param>
    /// <returns>The name of the migration assembly.</returns>
    public static string? GetMigrationAssemblyByProvider(DatabaseProviderType databaseProviderType)
    {
        return databaseProviderType switch
        {
            DatabaseProviderType.SqlServer => typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            DatabaseProviderType.MySql => typeof(MySqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            _ => throw new ArgumentOutOfRangeException(nameof(databaseProviderType), databaseProviderType, "Unsupported database provider type.")
        };
    }
}
