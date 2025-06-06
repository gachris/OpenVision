using Microsoft.AspNetCore.Mvc;
using OpenVision.IdentityServer.STS.Identity.Configuration.Interfaces;

namespace OpenVision.IdentityServer.STS.Identity.ViewComponents;

public class IdentityServerAdminLinkViewComponent : ViewComponent
{
    private readonly IRootConfiguration _configuration;

    public IdentityServerAdminLinkViewComponent(IRootConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IViewComponentResult Invoke()
    {
        var identityAdminUrl = _configuration.AdminConfiguration.IdentityAdminBaseUrl;

        return View(model: identityAdminUrl);
    }
}
