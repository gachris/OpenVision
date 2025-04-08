using MediatR;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Represents a command to delete a database.
/// </summary>
/// <param name="DatabaseId">The unique identifier of the database to delete.</param>
/// <returns>A boolean value indicating whether the deletion was successful.</returns>
public record DeleteDatabaseCommand(Guid DatabaseId) : IRequest<bool>;