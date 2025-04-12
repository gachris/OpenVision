using AutoMapper;
using Microsoft.Extensions.Logging;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL query resolver for database, target and trackable operations.
/// </summary>
public partial class Query
{
    #region Fields/Consts

    private readonly IMapper _mapper;
    private readonly ILogger<Query> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Query"/> class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public Query(IMapper mapper, ILogger<Query> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Executes the provided asynchronous function within a try-catch block to log and rethrow errors.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <returns>The result of the function.</returns>
    /// <exception cref="Exception">Rethrows any caught exceptions.</exception>
    private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the GraphQL query.");
            throw;
        }
    }

    #endregion
}