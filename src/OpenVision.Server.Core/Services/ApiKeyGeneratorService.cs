using System.Security.Cryptography;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for generating unique API keys.
/// </summary>
public class ApiKeyGeneratorService : IApiKeyGeneratorService
{
    #region Fields/Consts

    /// <summary>
    /// The length of the generated API keys in bytes.
    /// </summary>
    private const int ApiKeyLength = 32;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyGeneratorService"/> class.
    /// </summary>
    public ApiKeyGeneratorService()
    {
    }

    #region Methods

    /// <inheritdoc/>
    public string GenerateKey()
    {
        // Create a new instance of the RandomNumberGenerator class
        using var random = RandomNumberGenerator.Create();

        // Create a buffer to hold the random bytes
        var keyBytes = new byte[ApiKeyLength];

        // Fill the buffer with random bytes
        random.GetBytes(keyBytes);

        // Convert the random bytes to a string
        var apiKey = Convert.ToBase64String(keyBytes);

        // If the key is unique, return it
        return apiKey;
    }

    #endregion
}
