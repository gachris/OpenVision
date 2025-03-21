namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service interface for generating unique API keys.
/// </summary>
public interface IApiKeyGeneratorService
{
    /// <summary>
    /// Generates a unique API key.
    /// </summary>
    /// <returns>The generated API key.</returns>
    Task<string> GenerateAsync();
}