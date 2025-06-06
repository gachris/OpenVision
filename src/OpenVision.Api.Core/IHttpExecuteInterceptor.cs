﻿namespace OpenVision.Api.Core;

/// <summary>
/// HTTP request execute interceptor to intercept a <see cref="HttpRequestMessage" /> before it has
/// been sent. Sample usage is attaching "Authorization" header to a request.
/// </summary>
public interface IHttpExecuteInterceptor
{
    /// <summary>
    /// Invoked before the request is being sent.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    Task InterceptAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}