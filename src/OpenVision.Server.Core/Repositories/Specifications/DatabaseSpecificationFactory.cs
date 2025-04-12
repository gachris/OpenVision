using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// A factory for creating database specifications based on the current authentication context.
/// </summary>
public class DatabaseSpecificationFactory : IDatabaseSpecificationFactory
{
    #region Fields/Consts

    /// <summary>
    /// The service providing details about the current user's authentication context.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSpecificationFactory"/> class.
    /// </summary>
    /// <param name="currentUserService">The service that provides details about the current user's authentication context.</param>
    public DatabaseSpecificationFactory(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    #region Methods

    /// <summary>
    /// Creates a database specification based solely on the current authentication context.
    /// For a user-based context, returns a specification that filters by the current user's ID.
    /// For an API key-based context, returns a specification that filters by the provided API key.
    /// </summary>
    /// <returns>
    /// An <see cref="ISpecification{Database}"/> for retrieving databases associated with the current authentication context.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication context is available.</exception>
    public ISpecification<Database> GetDatabaseSpecification()
    {
        if (!string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return new DatabaseForUserSpecification(_currentUserService.UserId);
        }
        else if (!string.IsNullOrWhiteSpace(_currentUserService.ApiKey))
        {
            return new DatabaseForServerApiKeySpecification(_currentUserService.ApiKey)
            {
                Includes = { database => database.ApiKeys }
            };
        }
        else
        {
            throw new InvalidOperationException("No valid authentication context found.");
        }
    }

    /// <summary>
    /// Creates a specification for retrieving a specific database based on the provided database identifier and the current authentication context.
    /// For a user-based context, returns a specification that filters by the current user's ID along with the given database identifier.
    /// For an API key-based context, returns a specification that filters by the provided API key.
    /// </summary>
    /// <param name="databaseId">The unique identifier of the database to be retrieved.</param>
    /// <returns>
    /// An <see cref="ISpecification{Database}"/> that encapsulates the criteria for retrieving the specified database along with the required includes.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication context is available.</exception>
    public ISpecification<Database> GetDatabaseSpecification(Guid databaseId)
    {
        if (!string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return new DatabaseForUserSpecification(databaseId, _currentUserService.UserId);
        }
        else if (!string.IsNullOrWhiteSpace(_currentUserService.ApiKey))
        {
            return new DatabaseForServerApiKeySpecification(_currentUserService.ApiKey)
            {
                Includes = { database => database.ApiKeys }
            };
        }
        else
        {
            throw new InvalidOperationException("No valid authentication context found.");
        }
    }

    #endregion
}
