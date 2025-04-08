namespace OpenVision.Client.Core.Constants;

/// <summary>
/// Constants related to authentication.
/// </summary>
public class AuthenticationConsts
{
    /// <summary>
    /// The name of the cookie-based authentication scheme.
    /// </summary>
    public const string SignInScheme = "Cookies";

    /// <summary>
    /// The name of the OpenID Connect authentication scheme.
    /// </summary>
    public const string OidcAuthenticationScheme = "oidc";

    /// <summary>
    /// The path to the account login page.
    /// </summary>
    public const string AccountLoginPage = "Account/Login";
}