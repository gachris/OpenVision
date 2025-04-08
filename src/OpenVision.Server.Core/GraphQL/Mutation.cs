using AutoMapper;
using Microsoft.Extensions.Logging;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL mutation resolver for target operations.
/// </summary>
public partial class Mutation
{
    #region Fields/Consts

    private readonly IMapper _mapper;
    private readonly ILogger<Mutation> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Mutation"/> class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public Mutation(
        IMapper mapper,
        ILogger<Mutation> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// An empty GraphQL mutation that returns an empty string.
    /// </summary>
    /// <returns>A <see cref="Task{String}"/> representing the asynchronous operation, with an empty string as its result.</returns>
    public async Task<string> EmptyMutationAsync()
    {
        return await ExecuteAsync(async () =>
        {
            // Return an empty string as a placeholder result.
            return await Task.FromResult(string.Empty);
        });
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Executes the provided asynchronous function within a try-catch block to log and rethrow errors.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <returns>The result of the function.</returns>
    /// <exception cref="Exception">Rethrows any caught exception after logging it.</exception>
    private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing a target mutation.");
            throw;
        }
    }

    #endregion
}
