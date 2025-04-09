using OpenVision.EntityFramework.Entities;
using OpenVision.Shared;

namespace OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// Represents a specification for filtering <see cref="ImageTarget"/> entities based on a server API key.
/// This specification allows filtering by a server API key alone or by a combination of a target identifier and a server API key.
/// </summary>
public class ImageTargetForServerApiKeySpecification : BaseSpecification<ImageTarget>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetForServerApiKeySpecification"/> class 
    /// with a filter that selects image targets whose associated database's server API key matches the specified key.
    /// </summary>
    /// <param name="apiKey">The server API key used for filtering.</param>
    public ImageTargetForServerApiKeySpecification(string apiKey)
        : base(target => target.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTargetForServerApiKeySpecification"/> class 
    /// with a filter that selects a specific image target by its identifier and whose associated database's server API key matches the specified key.
    /// </summary>
    /// <param name="targetId">The unique identifier of the image target.</param>
    /// <param name="apiKey">The server API key used for filtering.</param>
    public ImageTargetForServerApiKeySpecification(Guid targetId, string apiKey)
        : base(target => target.Id == targetId
                  && target.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey
                  && target.Database.UserId == apiKey)
    {
    }
}
