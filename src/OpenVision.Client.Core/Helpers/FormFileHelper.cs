using Microsoft.AspNetCore.Http;

namespace OpenVision.Client.Core.Helpers;

/// <summary>
/// Provides helper methods for working with <see cref="IFormFile"/>.
/// </summary>
public class FormFileHelper
{
    /// <summary>
    /// Reads the contents of the specified form file and returns them as a byte array.
    /// </summary>
    /// <param name="image">The form file to convert. If <c>null</c>, <c>null</c> is returned.</param>
    /// <returns>
    /// A byte array containing the file data if the file is not <c>null</c>; otherwise, <c>null</c>.
    /// </returns>
    public static byte[]? GetAsByteArray(IFormFile? image)
    {
        if (image is null)
        {
            return null;
        }

        byte[] imageData;
        using var ms = new MemoryStream();
        image.CopyTo(ms);
        imageData = ms.ToArray();

        return imageData;
    }

    public static string? GetAsMetadata(IFormFile? file)
    {
        if (file is null)
        {
            return null;
        }

        using var sr = new StreamReader(file.OpenReadStream());
        return sr.ReadToEnd();
    }
}
