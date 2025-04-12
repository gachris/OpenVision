using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenVision.Core.Reco.DataTypes.Requests;
using OpenVision.Core.Reco.DataTypes.Responses;
using OpenVision.Shared.Extensions;
using OpenVision.Shared.Types;

namespace OpenVision.Core.Reco;

/// <summary>
/// Represents a client for cloud-based feature recognition using WebSocket communication.
/// </summary>
public class CloudRecognition : ICloudRecognition
{
    #region Fields/Consts

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    private const int ChunkSize = 4 * 1024;
    private const string ApiKeyHeader = "X-API-KEY";

    private ClientWebSocket? _client;
    private bool _isReady;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsReady => _isReady;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudRecognition"/> class.
    /// </summary>
    public CloudRecognition()
    {
    }

    #region ICloudReco Implementation

    /// <inheritdoc/>
    public async Task InitAsync(string apiKey)
    {
        _isReady = false;

        _client?.Dispose();
        _client = new ClientWebSocket();

        _client.Options.SetRequestHeader(ApiKeyHeader, apiKey);

        await _client.ConnectAsync(new Uri(VisionSystemConfig.WebSocketUrl), CancellationToken.None);

        _isReady = true;
    }

    #endregion

    #region Method Override

    /// <inheritdoc/>
    public FeatureMatchingResult Match(IImageRequest request)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(IsReady, false, nameof(IsReady));
        ArgumentNullException.ThrowIfNull(_client, nameof(_client));

        if (request.Mat.IsEmpty())
        {
            return new FeatureMatchingResult([]);
        }

        var frameId = $"frame_{DateTime.Now:ddMMyyyyhhmmss}";

        var matchRequest = new WSMatchRequest(
            frameId,
            request.Mat,
            request.OriginalWidth,
            request.OriginalHeight,
            request.IsGrayscale,
            request.IsLowResolution,
            request.HasRoi,
            request.HasGaussianBlur);

        var requestMessage = JsonSerializer.Serialize(matchRequest, JsonSerializerOptions);
        var buffer = Encoding.UTF8.GetBytes(requestMessage);

        // Send the message to the server
        _client.SendMessageInChunksAsync(buffer, WebSocketMessageType.Binary, ChunkSize, CancellationToken.None)
               .ConfigureAwait(true)
               .GetAwaiter()
               .GetResult();

        // Read data from the server
        var received = _client.ReceiveFullMessageAsync(ChunkSize, CancellationToken.None)
                              .ConfigureAwait(true)
                              .GetAwaiter()
                              .GetResult();

        // Convert the byte array to a string
        var receivedMessage = Encoding.UTF8.GetString(received.Buffer, 0, received.Count);

        var featureMatchingResponse = JsonSerializer.Deserialize<FeatureMatchingResponse>(receivedMessage, JsonSerializerOptions);
        ArgumentNullException.ThrowIfNull(featureMatchingResponse, nameof(featureMatchingResponse));

        if (featureMatchingResponse.StatusCode is not StatusCode.Success)
        {
            return new FeatureMatchingResult([]);
        }

        var matches = featureMatchingResponse.Response.Result.Select(targetMatchResponse =>
            new TargetMatchResult(
                targetMatchResponse.Id,
                targetMatchResponse.ProjectedRegion,
                targetMatchResponse.CenterX,
                targetMatchResponse.CenterY,
                targetMatchResponse.Angle,
                targetMatchResponse.Size,
                targetMatchResponse.HomographyArray))
            .ToArray();

        return new FeatureMatchingResult(matches);
    }

    #endregion
}
