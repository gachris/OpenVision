using OpenVision.Shared;

namespace OpenVision.Server.Core.GraphQL.Types;

/// <summary>
/// Represents a data transfer object for a database entity.
/// </summary>
[GraphQLDescription("Represents a database entity including its identifier, name, type, associated API keys, targets, and timestamps.")]
public record Database
{
    /// <summary>
    /// Gets or sets the unique identifier of the database.
    /// </summary>
    [ID]
    [GraphQLDescription("The unique identifier of the database.")]
    public required virtual Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [GraphQLDescription("The name of the database.")]
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets or sets the type of the database (device or cloud).
    /// </summary>
    [GraphQLDescription("The type of the database (device or cloud).")]
    public required virtual DatabaseType Type { get; init; }

    /// <summary>
    /// Gets or sets the date and time that the database was created.
    /// </summary>
    [GraphQLDescription("The date and time when the database was created.")]
    public required virtual DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets or sets the date and time that the database was last updated.
    /// </summary>
    [GraphQLDescription("The date and time when the database was last updated.")]
    public required virtual DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets or sets the collection of API keys associated with the database.
    /// </summary>
    [GraphQLDescription("The collection of API keys associated with the database.")]
    public required virtual ICollection<ApiKey> ApiKeys { get; init; }

    /// <summary>
    /// Gets or sets the collection of targets associated with the database.
    /// </summary>
    [GraphQLDescription("The collection of targets associated with the database.")]
    public required virtual ICollection<Target> Targets { get; init; }
}
