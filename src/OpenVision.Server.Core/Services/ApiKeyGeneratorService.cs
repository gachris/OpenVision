using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OpenVision.Server.EntityFramework.DbContexts;

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

    /// <summary>
    /// The application database context for interacting with the API keys table.
    /// </summary>
    private readonly ApplicationDbContext _applicationContext;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyGeneratorService"/> class.
    /// </summary>
    /// <param name="applicationContext">The application database context for interacting with the API keys table.</param>
    public ApiKeyGeneratorService(ApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    #region IApiKeyGeneratorService Implementation

    /// <inheritdoc/>
    public async Task<string> GenerateAsync()
    {
        while (true)
        {
            // Create a new instance of the RandomNumberGenerator class
            using var random = RandomNumberGenerator.Create();

            // Create a buffer to hold the random bytes
            var keyBytes = new byte[ApiKeyLength];

            // Fill the buffer with random bytes
            random.GetBytes(keyBytes);

            // Convert the random bytes to a string
            var apiKey = Convert.ToBase64String(keyBytes);

            // Check whether the key already exists in the database
            var keyExists = await _applicationContext.ApiKeys.AnyAsync(k => k.Key == apiKey);

            if (!keyExists)
            {
                // If the key is unique, return it
                return apiKey;
            }
        }
    }

    #endregion
}
