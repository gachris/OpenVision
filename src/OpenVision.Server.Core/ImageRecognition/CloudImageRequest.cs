using Emgu.CV;
using OpenVision.Core.DataTypes;

namespace OpenVision.Server.Core.ImageRecognition;

/// <inheritdoc/>
internal record CloudImageRequest : IImageRequest
{
    /// <inheritdoc/>
    public virtual required string Id { get; init; }

    /// <inheritdoc/>
    public virtual required Mat Mat { get; init; }

    /// <inheritdoc/>
    public virtual required int OriginalWidth { get; init; }

    /// <inheritdoc/>
    public virtual required int OriginalHeight { get; init; }

    /// <inheritdoc/>
    public virtual required bool IsGrayscale { get; init; }

    /// <inheritdoc/>
    public virtual required bool IsLowResolution { get; init; }

    /// <inheritdoc/>
    public virtual required bool HasRoi { get; init; }

    /// <inheritdoc/>
    public virtual required bool HasGaussianBlur { get; init; }
}