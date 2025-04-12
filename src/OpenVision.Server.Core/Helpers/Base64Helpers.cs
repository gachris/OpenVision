using System.Text.RegularExpressions;

namespace OpenVision.Server.Core.Helpers;

/// <summary>
/// Provides helper methods for working with Base64-encoded image data.
/// </summary>
internal static partial class Base64Helpers
{
    /// <summary>
    /// Converts a Base64 image string into a byte array.
    /// </summary>
    /// <param name="image">
    /// A string representing an image encoded in Base64 format. This string may include a Data URI 
    /// prefix (e.g., "data:image/jpeg;base64,").
    /// </param>
    /// <returns>
    /// A byte array representing the decoded image data, or <c>null</c> if the input string is null or empty.
    /// </returns>
    public static byte[]? GetAsByteArray(string? image)
    {
        // If the provided image string is null or empty, return null.
        if (string.IsNullOrEmpty(image))
        {
            return null;
        }

        // Use the regular expression to remove the Data URI scheme from the string (if present).
        var imageData = Base64ImageRegex().Replace(image, string.Empty);
        // Convert the resulting Base64 string into a byte array.
        return Convert.FromBase64String(imageData);
    }

    /// <summary>
    /// Returns a regular expression that matches the Base64 image Data URI prefix.
    /// </summary>
    /// <remarks>
    /// The regex pattern matches strings starting with "data:image/[a-zA-Z]+;base64,".
    /// This is used to strip the data header from a Base64-encoded image string.
    /// </remarks>
    [GeneratedRegex("^data:image/[a-zA-Z]+;base64,")]
    private static partial Regex Base64ImageRegex();
}