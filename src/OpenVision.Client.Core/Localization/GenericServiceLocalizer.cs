using System.Reflection;
using Microsoft.Extensions.Localization;

namespace OpenVision.Client.Core.Localization;

/// <summary>
/// A generic string localizer for controller classes.
/// </summary>
/// <typeparam name="TResourceSource">The type of the resource class.</typeparam>
public class GenericControllerLocalizer<TResourceSource> : IGenericControllerLocalizer<TResourceSource>
{
    private readonly IStringLocalizer _localizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericControllerLocalizer{TResourceSource}"/> class.
    /// </summary>
    /// <param name="factory">The string localizer factory to use.</param>
    public GenericControllerLocalizer(IStringLocalizerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var type = typeof(TResourceSource);
        var assemblyName = type.GetTypeInfo().Assembly.GetName().Name!;
        var typeName = type.Name.Remove(type.Name.IndexOf('`'));
        var baseName = (type.Namespace + "." + typeName)[assemblyName.Length..].Trim('.');

        _localizer = factory.Create(baseName, assemblyName);
    }

    /// <inheritdoc />
    public virtual LocalizedString this[string name] => name == null ? throw new ArgumentNullException(nameof(name)) : _localizer[name];

    /// <inheritdoc />
    public virtual LocalizedString this[string name, params object[] arguments] => name == null ? throw new ArgumentNullException(nameof(name)) : _localizer[name, arguments];

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}