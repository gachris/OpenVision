namespace OpenVision.Core.Features2d.Factory;

internal class FeatureMatcherFactory
{
    private static readonly Dictionary<FeatureMatcherType, Type> _featureMatcherFactories = new()
    {
        { FeatureMatcherType.Flannbased, typeof(FlannbasedFactory) },
        { FeatureMatcherType.BFMatcher, typeof(BFMatcherFactory) }
    };

    /// <summary>
    /// Creates a new instance of the factory for the specified feature matcher type.
    /// </summary>
    /// <param name="featureMatcherType">The type of feature matcher.</param>
    /// <returns>A new instance of the appropriate feature matcher factory.</returns>
    public static IFeatureMatcherFactory Create(FeatureMatcherType featureMatcherType)
    {
        if (_featureMatcherFactories.TryGetValue(featureMatcherType, out var factoryType))
        {
            return (IFeatureMatcherFactory)Activator.CreateInstance(factoryType)!;
        }

        throw new ArgumentOutOfRangeException(nameof(featureMatcherType), "Unsupported feature matcher type.");
    }
}