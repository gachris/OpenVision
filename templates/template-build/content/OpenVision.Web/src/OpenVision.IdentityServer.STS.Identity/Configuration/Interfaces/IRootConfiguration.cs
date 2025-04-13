using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

namespace OpenVision.IdentityServer.STS.Identity.Configuration.Interfaces;

public interface IRootConfiguration
{
    AdminConfiguration AdminConfiguration { get; }

    RegisterConfiguration RegisterConfiguration { get; }
}
