using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

namespace OpenVision.IdentityServer.STS.Identity.Helpers.Localization;

public static class LoginPolicyResolutionLocalizer
{
    public static string GetUserNameLocalizationKey(LoginResolutionPolicy policy)
    {
        return policy switch
        {
            LoginResolutionPolicy.Username => "Username",
            LoginResolutionPolicy.Email => "Email",
            _ => "Username",
        };
    }
}
