using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;

namespace OpenVision.Client.Core.Configuration;

public class SecurityConfiguration
{
    public List<string> CspTrustedDomains { get; set; } = [];

    public bool UseDeveloperExceptionPage { get; set; }

    public bool UseHsts { get; set; } = true;

    public Action<HstsOptions>? HstsConfigureAction { get; set; }

    public Action<AuthenticationBuilder>? AuthenticationBuilderAction { get; set; }

    public Action<AuthorizationOptions>? AuthorizationConfigureAction { get; set; }
}