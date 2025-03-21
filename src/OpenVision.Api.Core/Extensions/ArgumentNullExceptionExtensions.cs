using System.Diagnostics.CodeAnalysis;

namespace OpenVision.Api.Core.Extensions;

/// <summary>
/// Provides extension methods for performing argument validation checks.
/// </summary>
internal static class ArgumentNullExceptionExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to check for null or empty.</param>
    /// <param name="paramName">The name of the parameter.</param>
    public static void ThrowIfNullOrEmpty([NotNull] this string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Parameter was empty", paramName);
        }
    }
}