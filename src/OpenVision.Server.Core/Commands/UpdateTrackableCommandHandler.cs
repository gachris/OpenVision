using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Core.Configuration;
using OpenVision.Core.Features2d;
using OpenVision.Core.Utils;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Helpers;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the UpdateTrackableCommand by updating the target details.
/// </summary>
public partial class UpdateTrackableCommandHandler : IRequestHandler<UpdateTrackableCommand, TargetRecordDto>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTrackableCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTrackableCommandHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational and error messages.</param>
    public UpdateTrackableCommandHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<UpdateTrackableCommandHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the UpdateTrackableCommand request by updating the target details.
    /// </summary>
    /// <param name="request">The command containing the target ID and update details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="TargetRecordDto"/> representing the updated target.</returns>
    public async Task<TargetRecordDto> Handle(UpdateTrackableCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;
        _logger.LogInformation("Updating target {TargetId} for user {UserId}", request.TargetId, apiKey);

        var imageTargetForServerApiKeySpecification = new ImageTargetForServerApiKeySpecification(request.TargetId, apiKey)
        {
            Includes =
            {
                target => target.Database,
                target => target.Database.ApiKeys
            }
        };
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForServerApiKeySpecification, cancellationToken);
        var imageTarget = imageTargets.SingleOrDefault();

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found", request.TargetId);
            throw new ArgumentException("Target not found.");
        }

        var updateDto = request.UpdateTrackableDto;

        // Update basic fields.
        imageTarget.Name = updateDto.Name ?? imageTarget.Name;
        imageTarget.Width = updateDto.Width ?? imageTarget.Width;

        // Process new image if provided.
        if (updateDto.Image is not null && updateDto.Image.Length > 0)
        {
            var imageData = Base64ImageRegex().Replace(updateDto.Image, string.Empty);
            var buffer = Convert.FromBase64String(imageData);
            var image = buffer.ToMat();
            image.LowResolution(320);

            var imageRequest = VisionSystemConfig.ImageRequestBuilder.Build(image);
            var featureExtractor = new FeatureExtractor();
            var imageDetectionInfo = featureExtractor.DetectAndCompute(imageRequest);

            imageTarget.PreprocessImage = image.ToArray();
            imageTarget.AfterProcessImage = imageDetectionInfo.Mat.ToArray();
            imageTarget.AfterProcessImageWithKeypoints = Features2dHelper.DrawKeypoints(
                imageDetectionInfo.Mat,
                imageDetectionInfo.Keypoints,
                System.Drawing.Color.Red).ToArray();
            imageTarget.Keypoints = imageDetectionInfo.Keypoints.ToByteArray();
            imageTarget.Descriptors = imageDetectionInfo.Descriptors.DescriptorToArray();
            imageTarget.DescriptorsRows = imageDetectionInfo.Descriptors.Rows;
            imageTarget.DescriptorsCols = imageDetectionInfo.Descriptors.Cols;

            // Recalculate height based on the new image dimensions.
            imageTarget.Height = UnitsHelper.CalculateYUnits(imageTarget.Width, image.Width, image.Height);
        }
        else if (updateDto.Width.HasValue)
        {
            // Recalculate height using the existing preprocessed image.
            var image = imageTarget.PreprocessImage.ToMat();
            imageTarget.Height = UnitsHelper.CalculateYUnits(updateDto.Width.Value, image.Width, image.Height);
        }

        // Update additional fields.
        imageTarget.ActiveFlag = updateDto.ActiveFlag ?? imageTarget.ActiveFlag;
        imageTarget.Metadata = string.IsNullOrWhiteSpace(updateDto.Metadata) ? null : updateDto.Metadata;
        imageTarget.Updated = DateTimeOffset.Now;

        // Persist changes.
        await _imageTargetsRepository.UpdateAsync(imageTarget, cancellationToken);

        _logger.LogInformation("Updated target {TargetId} for user {UserId}", request.TargetId, apiKey);

        // Map the updated entity to a DTO and return.
        return _mapper.Map<TargetRecordDto>(imageTarget);
    }

    [GeneratedRegex("^data:image/[a-zA-Z]+;base64,")]
    private static partial Regex Base64ImageRegex();

    #endregion
}