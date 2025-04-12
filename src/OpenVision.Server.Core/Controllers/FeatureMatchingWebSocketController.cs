using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Core.Reco.DataTypes.Requests;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Shared.Extensions;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for handling WebSocket connections for feature matching.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = AuthorizationConsts.ClientApiKeyPolicy)]
public class FeatureMatchingWebSocketController : ControllerBase
{
    #region Fields/Consts

    private const int _chunkSize = 4 * 1024;

    private readonly IImageRecognitionManager _imageRecognitionManager;
    private readonly ILogger<FeatureMatchingWebSocketController> _logger;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureMatchingWebSocketController"/> class.
    /// </summary>
    /// <param name="imageRecognitionManager">The image recognition manager service.</param>
    /// <param name="logger">Logger instance for logging.</param>
    public FeatureMatchingWebSocketController(IImageRecognitionManager imageRecognitionManager, ILogger<FeatureMatchingWebSocketController> logger)
    {
        _imageRecognitionManager = imageRecognitionManager;
        _logger = logger;
    }

    /// <summary>
    /// Handles incoming WebSocket connections.
    /// </summary>
    [Route("/ws")]
    public async Task ConnectWebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await ProcessWebSocketSession(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    /// <summary>
    /// Handles the communication with the WebSocket client.
    /// </summary>
    /// <param name="webSocket">The WebSocket instance representing the client connection.</param>
    private async Task ProcessWebSocketSession(WebSocket webSocket)
    {
        _logger.LogTrace(nameof(ProcessWebSocketSession));

        await _imageRecognitionManager.InitializeAsync();

        var receiveResult = await webSocket.ReceiveFullMessageAsync(_chunkSize, CancellationToken.None);

        try
        {
            while (!receiveResult.CloseStatus.HasValue)
            {
                var receivedMessage = Encoding.UTF8.GetString(receiveResult.Buffer, 0, receiveResult.Count);
                var matchRequest = JsonSerializer.Deserialize<WSMatchRequest>(receivedMessage, JsonSerializerOptions);

                ArgumentNullException.ThrowIfNull(matchRequest, nameof(matchRequest));

                var featureMatchingResponse = _imageRecognitionManager.MatchFeatures(matchRequest);
                var stringMessage = JsonSerializer.Serialize(featureMatchingResponse, JsonSerializerOptions);
                var message = Encoding.UTF8.GetBytes(stringMessage);

                await webSocket.SendMessageInChunksAsync(message, WebSocketMessageType.Binary, _chunkSize, CancellationToken.None);
                receiveResult = await webSocket.ReceiveFullMessageAsync(_chunkSize, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {Message}", ex.Message);
        }
        finally
        {
            // Close the client connection
            await webSocket.CloseAsync(
                receiveResult.CloseStatus ?? WebSocketCloseStatus.Empty,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

            _logger.LogTrace("Client disconnected.");
        }
    }
}
