using OpenVision.Shared;

namespace OpenVision.Server.Core.GraphQL.Types;

/// <summary>
/// Represents a data transfer object for an API key.
/// </summary>
[GraphQLDescription("Represents an API key including its identifier, key value, type, timestamps, and its associated database.")]
public record ApiKey
{
    /// <summary>
    /// Gets or sets the unique identifier of the API key.
    /// </summary>
    [ID]
    [GraphQLDescription("The unique identifier of the API key.")]
    public required virtual Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the database that the api key belongs to.
    /// </summary>
    [GraphQLDescription("The unique identifier of the database to which the api key belongs.")]
    public required virtual Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets or sets the API key string.
    /// </summary>
    [GraphQLDescription("The API key string.")]
    public required virtual string Key { get; init; }

    /// <summary>
    /// Gets or sets the type of the API key (client or server).
    /// </summary>
    [GraphQLDescription("The type of the API key (client or server).")]
    public required virtual ApiKeyType Type { get; init; }

    /// <summary>
    /// Gets or sets the date and time that the API key was last updated.
    /// </summary>
    [GraphQLDescription("The date and time when the API key was last updated.")]
    public required virtual DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets or sets the date and time that the API key was created.
    /// </summary>
    [GraphQLDescription("The date and time when the API key was created.")]
    public required virtual DateTimeOffset Created { get; init; }
}
