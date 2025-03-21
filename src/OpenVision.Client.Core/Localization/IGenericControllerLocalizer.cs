using Microsoft.Extensions.Localization;

namespace OpenVision.Client.Core.Localization;

/// <summary>
/// Defines a generic string localizer for controller classes.
/// </summary>
/// <typeparam name="T">The type of the resource class.</typeparam>
public interface IGenericControllerLocalizer<out T>
{
    /// <summary>
    /// Gets a localized string for the specified key.
    /// </summary>
    /// <param name="name">The key to localize.</param>
    /// <returns>The localized string.</returns>
    LocalizedString this[string name] { get; }

    /// <summary>
    /// Gets a localized string for the specified key and arguments.
    /// </summary>
    /// <param name="name">The key to localize.</param>
    /// <param name="arguments">The arguments to format the string.</param>
    /// <returns>The localized string.</returns>
    LocalizedString this[string name, params object[] arguments] { get; }

    /// <summary>
    /// Gets all localized strings.
    /// </summary>
    /// <param name="includeParentCultures">Whether to include parent cultures.</param>
    /// <returns>The collection of localized strings.</returns>
    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);
}