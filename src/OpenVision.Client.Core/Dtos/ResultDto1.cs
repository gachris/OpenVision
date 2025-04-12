namespace OpenVision.Client.Core.Dtos;

/// <summary>
/// Represents the result of an operation along with data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data produced by the operation.</typeparam>
/// <param name="Data">The data returned by the operation.</param>
/// <param name="Error">An optional error message. If null or empty, the operation succeeded.</param>
/// <param name="Exception">An optional exception that occurred during the operation.</param>
public record ResultDto<T>(T Data, string? Error = null, Exception? Exception = null) : ResultDto(Error, Exception);
