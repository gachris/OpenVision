using System.Net.WebSockets;

namespace OpenVision.Shared.WebSockets;

/// <summary>
/// Represents the result of receiving a complete WebSocket message.
/// Inherits from <see cref="WebSocketReceiveResult"/> and includes the full message buffer.
/// </summary>
public class WebSocketCompleteMessageResult : WebSocketReceiveResult
{
    /// <summary>
    /// Gets the buffer containing the complete message data.
    /// </summary>
    public byte[] Buffer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketCompleteMessageResult"/> class.
    /// </summary>
    /// <param name="buffer">The buffer containing the complete message data.</param>
    /// <param name="messageType">The type of the WebSocket message.</param>
    /// <param name="endOfMessage">A value indicating whether this is the final message.</param>
    /// <param name="closeStatus">The WebSocket close status, if available.</param>
    /// <param name="closeStatusDescription">The WebSocket close status description, if available.</param>
    public WebSocketCompleteMessageResult(byte[] buffer,
                                          WebSocketMessageType messageType,
                                          bool endOfMessage,
                                          WebSocketCloseStatus? closeStatus,
                                          string? closeStatusDescription)
        : base(buffer.Length, messageType, endOfMessage, closeStatus, closeStatusDescription)
    {
        Buffer = buffer;
    }
}