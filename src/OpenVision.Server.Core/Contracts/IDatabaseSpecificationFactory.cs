using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides a contract for creating database specifications based on the current authentication context.
/// </summary>
public interface IDatabaseSpecificationFactory
{
    /// <summary>
    /// Creates a specification for retrieving a database using the current authentication context.
    /// This overload is used when no specific database identifier is required and may return a default or associated database.
    /// </summary>
    /// <returns>
    /// A specification for fetching the database with the necessary includes based on the current user's ID or API key.
    /// </returns>
    ISpecification<Database> GetDatabaseSpecification();

    /// <summary>
    /// Creates a specification for retrieving a specific database using the provided database identifier
    /// while taking into account the current authentication context (user ID or API key).
    /// </summary>
    /// <param name="databaseId">The unique identifier of the database to be retrieved.</param>
    /// <returns>
    /// A specification for fetching the specific database with the required includes.
    /// </returns>
    ISpecification<Database> GetDatabaseSpecification(Guid databaseId);
}
