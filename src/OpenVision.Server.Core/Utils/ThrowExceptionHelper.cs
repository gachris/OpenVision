using System.Diagnostics.CodeAnalysis;
using OpenVision.Shared;
using OpenVision.Shared.Exceptions;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Utils;

/// <summary>
/// Provides extension methods for throwing exceptions based on certain conditions.
/// </summary>
public static class ThrowExceptionHelper
{
    /// <summary>
    /// Throws an exception with the specified result code and message if the specified value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfNull<T>([NotNull] this T? value, ResultCode resultCode, string message)
    {
        if (value is null)
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified value is not equal to the specified value1.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="value1">The value to compare to.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfNotEquals<T>(this T? value, T? value1, ResultCode resultCode, string message) where T : struct
    {
        if (!value.Equals(value1))
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfNullOrEmpty([NotNull] this string? value, ResultCode resultCode, string message)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified value is false.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfFalse(this bool value, ResultCode resultCode, string message)
    {
        if (!value)
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified value is true.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfTrue(this bool value, ResultCode resultCode, string message)
    {
        if (value)
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an <see cref="HttpException"/> with the specified <paramref name="resultCode"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    /// <returns>The <see cref="HttpException"/> with the specified <paramref name="resultCode"/> and <paramref name="message"/>.</returns>
    private static HttpException ThrowHttpException(ResultCode resultCode, string message)
    {
        var errorCollection = new List<Error>();

        var error = new Error(resultCode, message);

        errorCollection.Add(error);

        var errorResponseMessage = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);

        return new HttpException(errorResponseMessage);
    }
}