using OpenVision.Api.Core;

namespace OpenVision.Api.Auth;

/// <summary>
/// Represents a credential that can be used to authenticate an HTTP client.
/// </summary>
public interface ICredential : IConfigurableHttpClientInitializer
{
}