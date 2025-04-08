using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// Provides common functionality for API controllers.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    #region Field/Consts

    protected readonly ILogger _logger;

    #endregion

    protected ApiControllerBase(ILogger logger)
    {
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Executes an asynchronous action within a try-catch block.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The resulting IActionResult.</returns>
    protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, Error("An internal server error occurred."));
        }
    }

    /// <summary>
    /// Creates a success response message with no result.
    /// </summary>
    /// <returns>A success response message with no result.</returns>
    protected static IResponseMessage Success()
    {
        return new ResponseMessage(Guid.NewGuid(), Shared.StatusCode.Success, []);
    }

    /// <summary>
    /// Creates an error response message.
    /// </summary>
    /// <returns>An error response message.</returns>
    protected static IResponseMessage Error(string message)
    {
        return new ResponseMessage(Guid.NewGuid(), Shared.StatusCode.Success, [new(ResultCode.InternalServerError, message)]);
    }

    /// <summary>
    /// Creates a success response message with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A success response message with the specified result.</returns>
    protected static IResponseMessage<TResult> Success<TResult>(TResult result)
    {
        return new ResponseMessage<TResult>(new ResponseDoc<TResult>(result), Guid.NewGuid(), Shared.StatusCode.Success, []);
    }

    #endregion
}