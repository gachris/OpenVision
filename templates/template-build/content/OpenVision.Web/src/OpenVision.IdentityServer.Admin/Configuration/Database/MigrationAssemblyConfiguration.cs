using System;
using System.Reflection;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using MySqlMigrationAssembly = OpenVision.IdentityServer.Admin.EntityFramework.MySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = OpenVision.IdentityServer.Admin.EntityFramework.PostgreSQL.Helpers.MigrationAssembly;
using SqlMigrationAssembly = OpenVision.IdentityServer.Admin.EntityFramework.SqlServer.Helpers.MigrationAssembly;

namespace OpenVision.IdentityServer.Admin.Configuration.Database;

public static class MigrationAssemblyConfiguration
{
    public static string GetMigrationAssemblyByProvider(DatabaseProviderConfiguration databaseProvider)
    {
        return databaseProvider.ProviderType switch
        {
            DatabaseProviderType.SqlServer => typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo()
                .Assembly.GetName()
                .Name,
            DatabaseProviderType.MySql => typeof(MySqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
