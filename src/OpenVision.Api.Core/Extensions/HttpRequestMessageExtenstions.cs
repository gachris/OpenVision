using System.Text;

namespace OpenVision.Api.Core.Extensions;

/// <summary>
/// Extension methods to <see cref="HttpRequestMessage"/>.
/// </summary>
internal static class HttpRequestMessageExtenstions
{
    /// <summary>
    /// Sets the content of the request by the given body.
    /// </summary>
    /// <param name="request">The HTTP request message to which to set the content.</param>
    /// <param name="service">The client service associated with the request.</param>
    /// <param name="body">The body of the request to set as the content. If <c>null</c>, the content is not set.</param>
    internal static void SetRequestSerailizedContent(this HttpRequestMessage request, IClientService service, object body)
    {
        if (body != null)
        {
            var mediaType = "application/" + service.Serializer.Format;
            var content = service.SerializeObject(body);
            request.Content = new StringContent(content, Encoding.UTF8, mediaType);
        }
    }
}
