using OpenVision.Api.Core;
using OpenVision.Api.Target.Resources;

namespace OpenVision.Api.Target.Services;

/// <summary>
/// The Target service.
/// </summary>
public class TargetService : BaseClientService
{
    /// <summary>
    /// Api version.
    /// </summary>
    public const string Version = "v1";

    /// <summary>
    /// Constructs a new service.
    /// </summary>
    public TargetService(Initializer initializer) : base(initializer)
    {
        BaseUri = initializer.ServerUrl;
        TargetList = new TargetListResource(this);
    }

    /// <inheritdoc/>
    public override string Name => "target";

    /// <inheritdoc/>
    public override string BaseUri { get; }

    /// <summary>
    /// Gets the TargetList resource.
    /// </summary>
    public virtual TargetListResource TargetList { get; }
}