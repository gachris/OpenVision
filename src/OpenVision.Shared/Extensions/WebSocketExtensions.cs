using System.Net.WebSockets;
using OpenVision.Shared.Responses;

namespace OpenVision.Shared.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="WebSocket"/>.
/// </summary>
public static class WebSocketExtensions
{
    /// <summary>
    /// Receives a complete message from the WebSocket as a single operation.
    /// </summary>
    /// <param name="webSocket">The <see cref="WebSocket"/> to receive the message from.</param>
    /// <param name="chunkSize">The size of the chunks to receive at a time.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="WebSocketCompleteMessageResult"/> containing the full message.</returns>
    public static async Task<WebSocketCompleteMessageResult> ReceiveFullMessageAsync(this WebSocket webSocket, int chunkSize, CancellationToken cancellationToken)
    {
        WebSocketReceiveResult receiveResult;
        var compoundBuffer = new List<byte>();
        var buffer = new byte[chunkSize];

        do
        {
            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            var readBytes = new byte[receiveResult.Count];
            Array.Copy(buffer, readBytes, receiveResult.Count);
            compoundBuffer.AddRange(readBytes);

        } while (!receiveResult.EndOfMessage && !receiveResult.CloseStatus.HasValue);

        return new WebSocketCompleteMessageResult([.. compoundBuffer], receiveResult.MessageType, true, receiveResult.CloseStatus, receiveResult.CloseStatusDescription);
    }

    /// <summary>
    /// Sends a message over the WebSocket in chunks.
    /// </summary>
    /// <param name="webSocket">The <see cref="WebSocket"/> to send the message through.</param>
    /// <param name="buffer">The buffer containing the message to send.</param>
    /// <param name="messageType">The type of the WebSocket message.</param>
    /// <param name="chunkSize">The size of the chunks to send at a time.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous send operation.</returns>
    public static async Task SendMessageInChunksAsync(this WebSocket webSocket, byte[] buffer, WebSocketMessageType messageType, int chunkSize, CancellationToken cancellationToken)
    {
        var dataLength = buffer.Length;
        var bytesSent = 0;

        while (bytesSent < dataLength)
        {
            var remainingBytes = dataLength - bytesSent;
            var bytesToSend = Math.Min(chunkSize, remainingBytes);
            var endOfMessage = bytesToSend == remainingBytes;

            await webSocket.SendAsync(new ArraySegment<byte>(buffer, bytesSent, bytesToSend),
                                      messageType,
                                      endOfMessage,
                                      cancellationToken);

            bytesSent += bytesToSend;
        }
    }
}
