using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// A factory for creating specifications for retrieving image targets based on the current authentication context.
/// </summary>
public class TargetSpecificationFactory : ITargetSpecificationFactory
{
    #region Fields/Consts

    /// <summary>
    /// The service that provides details about the current user's authentication context.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetSpecificationFactory"/> class.
    /// </summary>
    /// <param name="currentUserService">The service for obtaining the current user's authentication data.</param>
    public TargetSpecificationFactory(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    #region Methods

    /// <summary>
    /// Creates an image target specification based solely on the current authentication context.
    /// For a user-based context, returns a specification that filters by the current user's ID.
    /// For an API key-based context, returns a specification that filters by the provided API key.
    /// </summary>
    /// <returns>
    /// An <see cref="ISpecification{ImageTarget}"/> for retrieving image targets associated with the current authentication context.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication context is available.</exception>
    public ISpecification<ImageTarget> GetImageTargetSpecification()
    {
        if (!string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return new ImageTargetForUserSpecification(_currentUserService.UserId)
            {
                Includes = { target => target.Database }
            };
        }
        else if (!string.IsNullOrWhiteSpace(_currentUserService.ApiKey))
        {
            return new ImageTargetForServerApiKeySpecification(_currentUserService.ApiKey)
            {
                Includes =
                {
                    target => target.Database,
                    target => target.Database!.ApiKeys
                }
            };
        }
        else
        {
            throw new InvalidOperationException("No valid authentication context found.");
        }
    }

    /// <summary>
    /// Creates an image target specification based on the current authentication context and the specified target identifier.
    /// Returns a user-based specification if a user ID is present, otherwise returns an API key-based specification.
    /// </summary>
    /// <param name="targetId">The unique identifier for the image target.</param>
    /// <returns>
    /// An <see cref="ISpecification{ImageTarget}"/> that encapsulates the criteria for retrieving the specified image target.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication context is available.</exception>
    public ISpecification<ImageTarget> GetImageTargetSpecification(Guid targetId)
    {
        if (!string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return new ImageTargetForUserSpecification(targetId, _currentUserService.UserId)
            {
                Includes = { target => target.Database }
            };
        }
        else if (!string.IsNullOrWhiteSpace(_currentUserService.ApiKey))
        {
            return new ImageTargetForServerApiKeySpecification(targetId, _currentUserService.ApiKey)
            {
                Includes =
                {
                    target => target.Database,
                    target => target.Database!.ApiKeys
                }
            };
        }
        else
        {
            throw new InvalidOperationException("No valid authentication context found.");
        }
    }

    #endregion
}
