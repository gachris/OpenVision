namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides access to the current user's identifier.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the identifier of the current user.
    /// If the user identifier is not found, an exception is thrown.
    /// </summary>
    string UserId { get; }
}
