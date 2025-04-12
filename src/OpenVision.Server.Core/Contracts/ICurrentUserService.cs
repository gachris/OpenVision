namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides access to the current user's identifier.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the identifier of the current user.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current API key if provided in the user's claims.
    /// </summary>
    string? ApiKey { get; }
}
