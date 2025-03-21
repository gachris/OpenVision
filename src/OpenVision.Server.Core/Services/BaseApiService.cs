using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Abstract base class for API services providing common helper methods.
/// </summary>
public abstract class BaseApiService
{
    #region Helpers

    /// <summary>
    /// Creates a success response message with no result.
    /// </summary>
    /// <returns>A success response message with no result.</returns>
    protected static IResponseMessage Success()
    {
        return new ResponseMessage(Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <summary>
    /// Creates a success response message with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A success response message with the specified result.</returns>
    protected static IResponseMessage<TResult> Success<TResult>(TResult result)
    {
        return new ResponseMessage<TResult>(new(result), Guid.NewGuid(), StatusCode.Success, []);
    }

    #endregion
}