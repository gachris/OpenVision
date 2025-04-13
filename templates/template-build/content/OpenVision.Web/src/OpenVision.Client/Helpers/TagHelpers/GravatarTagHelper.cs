using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OpenVision.Client.Helpers.TagHelpers;

/// <summary>
/// A tag helper that generates a Gravatar image tag based on an email address.
/// </summary>
[HtmlTargetElement("img-gravatar")]
public class GravatarTagHelper : TagHelper
{
    /// <summary>
    /// The email address to generate the Gravatar image for.
    /// </summary>
    [HtmlAttributeName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// The alternate text for the image.
    /// </summary>
    [HtmlAttributeName("alt")]
    public string? Alt { get; set; }

    /// <summary>
    /// The CSS class for the image.
    /// </summary>
    [HtmlAttributeName("class")]
    public string? Class { get; set; }

    /// <summary>
    /// The size of the image.
    /// </summary>
    [HtmlAttributeName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Processes the Gravatar tag helper and generates the HTML.
    /// </summary>
    /// <param name="context">The tag helper context.</param>
    /// <param name="output">The tag helper output.</param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!string.IsNullOrWhiteSpace(Email))
        {
            var hash = Md5HashHelper.GetHash(Email);

            output.TagName = "img";
            if (!string.IsNullOrWhiteSpace(Class))
            {
                output.Attributes.Add("class", Class);
            }

            if (!string.IsNullOrWhiteSpace(Alt))
            {
                output.Attributes.Add("alt", Alt);
            }

            output.Attributes.Add("src", GetAvatarUrl(hash, Size));
            output.TagMode = TagMode.SelfClosing;
        }
    }

    /// <summary>
    /// Returns the URL of the Gravatar image for the specified email hash and size.
    /// </summary>
    /// <param name="hash">The MD5 hash of the email address.</param>
    /// <param name="size">The size of the image.</param>
    /// <returns>The URL of the Gravatar image.</returns>
    private static string GetAvatarUrl(string hash, int size)
    {
        var sizeArg = size > 0 ? $"?s={size}" : "";

        return $"https://www.gravatar.com/avatar/{hash}{sizeArg}";
    }
}
