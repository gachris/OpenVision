using MediatR;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Helpers;
using OpenVision.Server.Core.Properties;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Core.Utils;
using OpenVision.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Features2d;
using AutoMapper;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the UpdateTargetCommand by updating the target details.
/// </summary>
public class UpdateTargetCommandHandler : IRequestHandler<UpdateTargetCommand, TargetDto>
{
    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTargetCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational and error messages.</param>
    public UpdateTargetCommandHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<UpdateTargetCommandHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateTargetCommand request by updating the target details.
    /// </summary>
    /// <param name="request">The command containing the target ID and update details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="TargetDto"/> representing the updated target.</returns>
    public async Task<TargetDto> Handle(UpdateTargetCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("Updating target {TargetId} for user {UserId}", request.TargetId, userId);

        // Retrieve the target from the repository.
        var targetsQueryable = await _imageTargetsRepository.GetAsync();
        var target = await targetsQueryable.Where(x => x.Id == request.TargetId)
            .SingleOrDefaultAsync(cancellationToken);

        if (target is null)
        {
            _logger.LogWarning("Target {TargetId} not found", request.TargetId);
            throw new ArgumentException(ErrorMessages.TargetNotFound);
        }

        var updateDto = request.UpdateTargetDto;

        // Update basic fields.
        target.Name = updateDto.Name ?? target.Name;
        target.Width = updateDto.Width ?? target.Width;

        // Process new image if provided.
        if (updateDto.Image is not null && updateDto.Image.Length > 0)
        {
            var image = updateDto.Image.ToMat();
            image.LowResolution(320);

            var imageRequest = VisionSystemConfig.ImageRequestBuilder.Build(image);
            var featureExtractor = new FeatureExtractor();
            var imageDetectionInfo = featureExtractor.DetectAndCompute(imageRequest);

            target.PreprocessImage = image.ToArray();
            target.AfterProcessImage = imageDetectionInfo.Mat.ToArray();
            target.AfterProcessImageWithKeypoints = Features2dHelper.DrawKeypoints(
                imageDetectionInfo.Mat,
                imageDetectionInfo.Keypoints,
                System.Drawing.Color.Red).ToArray();
            target.Keypoints = imageDetectionInfo.Keypoints.ToByteArray();
            target.Descriptors = imageDetectionInfo.Descriptors.DescriptorToArray();
            target.DescriptorsRows = imageDetectionInfo.Descriptors.Rows;
            target.DescriptorsCols = imageDetectionInfo.Descriptors.Cols;

            // Recalculate height based on the new image dimensions.
            target.Height = UnitsHelper.CalculateYUnits(target.Width, image.Width, image.Height);
        }
        else if (updateDto.Width.HasValue)
        {
            // Recalculate height using the existing preprocessed image.
            var image = target.PreprocessImage.ToMat();
            target.Height = UnitsHelper.CalculateYUnits(updateDto.Width.Value, image.Width, image.Height);
        }

        // Update additional fields.
        target.ActiveFlag = updateDto.ActiveFlag ?? target.ActiveFlag;
        target.Metadata = string.IsNullOrWhiteSpace(updateDto.Metadata) ? null : updateDto.Metadata;
        target.Updated = DateTimeOffset.Now;

        // Persist changes.
        await _imageTargetsRepository.UpdateAsync(target, cancellationToken);

        _logger.LogInformation("Updated target {TargetId} for user {UserId}", request.TargetId, userId);

        // Map the updated entity to a DTO and return.
        return _mapper.Map<TargetDto>(target);
    }
}