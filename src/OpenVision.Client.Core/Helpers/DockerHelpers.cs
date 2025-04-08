using Microsoft.Extensions.Configuration;
using OpenVision.Client.Core.Configuration;

namespace OpenVision.Client.Core.Helpers;

/// <summary>
/// Helper methods for Docker-related operations.
/// </summary>
public class DockerHelpers
{
    /// <summary>
    /// Updates the CA certificates in the Docker environment.
    /// </summary>
    public static void UpdateCaCertificates()
    {
        "update-ca-certificates".Bash();
    }

    /// <summary>
    /// Applies Docker-related configuration settings.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    public static void ApplyDockerConfiguration(IConfiguration configuration)
    {
        var dockerConfiguration = configuration.GetSection(nameof(DockerConfiguration)).Get<DockerConfiguration>();

        if (dockerConfiguration != null && dockerConfiguration.UpdateCaCertificate)
        {
            UpdateCaCertificates();
        }
    }
}
