using OpenVision.IdentityServer.STS.Identity.Configuration.Interfaces;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

namespace OpenVision.IdentityServer.STS.Identity.Configuration;

public class RootConfiguration : IRootConfiguration
{
    public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
    public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
}
