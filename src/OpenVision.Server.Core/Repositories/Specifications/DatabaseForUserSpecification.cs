using OpenVision.EntityFramework.Entities;
using OpenVision.Shared;

namespace OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// Represents a specification for filtering <see cref="Database"/> entities based on user-specific criteria.
/// This specification allows filtering by user identifier alone or by a combination of a database identifier and a user identifier.
/// </summary>
public class DatabaseForUserSpecification : BaseSpecification<Database>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseForUserSpecification"/> class 
    /// with a filter that selects all database entities associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    public DatabaseForUserSpecification(string userId)
        : base(database => database.UserId == userId)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseForUserSpecification"/> class 
    /// with a filter that selects a specific database entity associated with the specified user.
    /// </summary>
    /// <param name="databaseId">The unique identifier of the database entity.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    public DatabaseForUserSpecification(Guid databaseId, string userId)
        : base(database => database.Id == databaseId && database.UserId == userId)
    {
    }
}

/// <summary>
/// Represents a specification for filtering <see cref="Database"/> entities based on user-specific criteria.
/// This specification allows filtering by user identifier alone or by a combination of a database identifier and a user identifier.
/// </summary>
public class DatabaseForServerApiKeySpecification : BaseSpecification<Database>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseForUserSpecification"/> class 
    /// with a filter that selects all database entities associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    public DatabaseForServerApiKeySpecification(string apiKey)
        : base(database => database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey)
    {
    }
}
