﻿using System.Reflection;

namespace OpenVision.EntityFramework.MySql.Helpers;

/// <summary>
/// Provides helper methods for retrieving migration assembly information for MySQL.
/// </summary>
public static class MigrationAssembly
{
    /// <summary>
    /// Retrieves the name of the migration assembly.
    /// </summary>
    /// <returns>The name of the migration assembly as a string.</returns>
    public static string? GetMigrationAssemblyName()
    {
        return typeof(MigrationAssembly).GetTypeInfo().Assembly.GetName().Name;
    }
}
