using OpenVision.Api.Core;
using OpenVision.Api.Core.Request;

namespace OpenVision.Api.Target.Requests;

/// <summary>
/// An abstract base class for service requests used in the vision targets.
/// </summary>
public abstract class TargetBaseServiceRequest<TResponse> : ClientServiceRequest<TResponse>
{
    /// <summary>
    /// Constructs a new instance of the <see cref="TargetBaseServiceRequest{TResponse}"/> class.
    /// </summary>
    /// <param name="service">The client service instance used to make the request.</param>
    protected TargetBaseServiceRequest(IClientService service) : base(service)
    {
    }
}
