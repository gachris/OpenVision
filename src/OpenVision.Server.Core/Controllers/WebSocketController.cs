using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Emgu.CV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Core.Dataset;
using OpenVision.Core.Reco;
using OpenVision.Core.Reco.DataTypes.Requests;
using OpenVision.Core.Reco.DataTypes.Responses;
using OpenVision.Core.Utils;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.EntityFramework.DbContexts;
using OpenVision.Shared;
using OpenVision.Shared.Responses;
using OpenVision.Shared.WebSockets;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// Api controller for handling WebSocket connections.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = AuthorizationConsts.ClientApiKeyPolicy)]
public class WebSocketController : ControllerBase
{
    #region Feilds/Consts

    private const int ChunkSize = 4 * 1024;

    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<WebSocketController> _logger;
    private readonly IImageRecognition _imageRecognition = new ImageRecognition();

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
    /// Initializes a new instance of the <see cref="WebSocketController"/> class.
    /// </summary>
    /// <param name="applicationDbContext">The application database context.</param>
    /// <param name="logger">Logger instance for logging.</param>
    public WebSocketController(ApplicationDbContext applicationDbContext, ILogger<WebSocketController> logger)
    {
        _dbContext = applicationDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles incoming WebSocket connections.
    /// </summary>
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await HandleClient(webSocket);
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
    private async Task HandleClient(WebSocket webSocket)
    {
        _logger.LogTrace(nameof(HandleClient));

        var clientApiKey = HttpContext.User.Claims.First(claim => claim.Type.Equals(ApiKeyDefaults.X_API_KEY)).Value;


        await _dbContext.Databases.Include(x => x.ApiKeys)
                                  .LoadAsync();

        var database = await _dbContext.Databases.FirstOrDefaultAsync(database => database.ApiKeys.Any(apiKey => apiKey.Key == clientApiKey && apiKey.Type == ApiKeyType.Client));

        ArgumentNullException.ThrowIfNull(database, nameof(database));

        await _dbContext.ImageTargets.Where(x => x.DatabaseId == database.Id && x.ActiveFlag == ActiveFlag.True)
            .LoadAsync();

        var targets = database.ImageTargets.Select(imageTarget => new Target(
              imageTarget.Id.ToString(),
              imageTarget.AfterProcessImage,
              imageTarget.Keypoints,
              imageTarget.Descriptors,
              imageTarget.DescriptorsRows,
              imageTarget.DescriptorsCols,
              imageTarget.Width,
              imageTarget.Height));

        await _imageRecognition.InitAsync(targets);

        var receiveResult = await webSocket.ReceiveFullMessageAsync(ChunkSize, CancellationToken.None);

        try
        {
            while (!receiveResult.CloseStatus.HasValue)
            {
                var receivedMessage = Encoding.ASCII.GetString(receiveResult.Buffer, 0, receiveResult.Count);
                var matchRequest = JsonSerializer.Deserialize<WSMatchRequest>(receivedMessage, JsonSerializerOptions);

                ArgumentNullException.ThrowIfNull(matchRequest, nameof(matchRequest));

                var cloudImageRequest = new CloudImageRequest(
                    matchRequest.Id,
                    matchRequest.Mat,
                    matchRequest.OriginalWidth,
                    matchRequest.OriginalHeight,
                    matchRequest.IsGrayscale,
                    matchRequest.IsLowResolution,
                    matchRequest.HasRoi,
                    matchRequest.HasGaussianBlur);

                var featureMatchingResult = _imageRecognition.Match(cloudImageRequest);

                var matches = featureMatchingResult.Matches.Select(match =>
                    new TargetMatchResponse(
                        match.Id,
                        match.ProjectedRegion,
                        match.Size,
                        match.CenterX,
                        match.CenterY,
                        match.Angle,
                        match.HomographyArray))
                    .ToArray();

                var featureMatchingResponse = new FeatureMatchingResponse(
                    new ResponseDoc<IReadOnlyCollection<TargetMatchResponse>>(matches),
                    Guid.NewGuid(),
                    Shared.StatusCode.Success,
                    []);

                var stringMessage = JsonSerializer.Serialize(featureMatchingResponse, JsonSerializerOptions);
                var message = Encoding.UTF8.GetBytes(stringMessage);

                // Send the message to the client
                await webSocket.SendMessageInChunksAsync(message, WebSocketMessageType.Binary, ChunkSize, CancellationToken.None);

                // Receive the message to the client
                receiveResult = await webSocket.ReceiveFullMessageAsync(ChunkSize, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
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

internal class MatConverter : JsonConverter<Mat>
{
    public MatConverter() : base()
    {
    }

    public override Mat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Ensure the token is a start of an array
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected a JSON array to deserialize Mat object.");
        }

        // Read byte array from JSON
        var data = new List<byte>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType != JsonTokenType.Number || !reader.TryGetByte(out byte value))
            {
                throw new JsonException("Expected byte values in the JSON array.");
            }

            data.Add(value);
        }

        var byteArray = data.ToArray();

        return byteArray.ToMat();
    }

    public override void Write(Utf8JsonWriter writer, Mat value, JsonSerializerOptions options)
    {
        // Serialize Mat as a byte array
        writer.WriteStartArray();

        // Convert the Mat object to a byte array
        var byteArray = value.ToArray();

        // Write each byte to the JSON array
        foreach (var b in byteArray)
        {
            writer.WriteNumberValue(b);
        }

        writer.WriteEndArray();
    }
}