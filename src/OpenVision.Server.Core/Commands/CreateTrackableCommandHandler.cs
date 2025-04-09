using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Core.Configuration;
using OpenVision.Core.Features2d;
using OpenVision.Core.Utils;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Helpers;
using OpenVision.Server.Core.Repositories.Specifications;
using OpenVision.Shared;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the CreateTrackableCommand by processing the image, extracting features,
/// and creating a new target associated with a database.
/// </summary>
public partial class CreateTrackableCommandHandler : IRequestHandler<CreateTrackableCommand, TargetRecordDto>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTrackableCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTrackableCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational and error messages.</param>
    public CreateTrackableCommandHandler(
        IDatabasesRepository databasesRepository,
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<CreateTrackableCommandHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the CreateTrackableCommand request by creating a new target.
    /// </summary>
    /// <param name="request">The command containing the target creation details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="TargetDto"/> representing the newly created target.</returns>
    public async Task<TargetRecordDto> Handle(CreateTrackableCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;
        var createDto = request.PostTrackableDto;
        var targetId = Guid.NewGuid();

        var databaseForServerApiKeySpecification = new DatabaseForServerApiKeySpecification(apiKey)
        {
            Includes = { database => database.ApiKeys }
        };
        var databases = await _databasesRepository.GetBySpecificationAsync(databaseForServerApiKeySpecification, cancellationToken);
        var database = databases.SingleOrDefault() ?? throw new ArgumentException("Database not found.");

        // Process the image.
        var imageData = Base64ImageRegex()
            .Replace(createDto.Image, string.Empty);
        var buffer = Convert.FromBase64String(imageData);
        var image = buffer.ToMat();
        image.LowResolution(320);

        var imageRequest = VisionSystemConfig.ImageRequestBuilder.Build(image);
        var featureExtractor = new FeatureExtractor();
        var imageDetectionInfo = featureExtractor.DetectAndCompute(imageRequest);

        var height = UnitsHelper.CalculateYUnits(createDto.Width, image.Width, image.Height);

        // Create the new target.
        var target = new ImageTarget
        {
            Id = targetId,
            DatabaseId = database.Id,
            Name = createDto.Name,
            Type = TargetType.Cloud,
            Width = createDto.Width,
            Height = height,
            ActiveFlag = createDto.ActiveFlag ?? ActiveFlag.True,
            Metadata = createDto.Metadata,
            PreprocessImage = image.ToArray(),
            AfterProcessImage = imageDetectionInfo.Mat.ToArray(),
            AfterProcessImageWithKeypoints = Features2dHelper.DrawKeypoints(
                imageDetectionInfo.Mat,
                imageDetectionInfo.Keypoints,
                System.Drawing.Color.Red).ToArray(),
            Keypoints = imageDetectionInfo.Keypoints.ToByteArray(),
            Descriptors = imageDetectionInfo.Descriptors.DescriptorToArray(),
            DescriptorsRows = imageDetectionInfo.Descriptors.Rows,
            DescriptorsCols = imageDetectionInfo.Descriptors.Cols,
            Created = DateTimeOffset.Now,
            Updated = DateTimeOffset.Now,
            Database = database,
            Rating = 5,
            Recos = 0
        };

        // Persist the new target.
        await _imageTargetsRepository.CreateAsync(target, cancellationToken);

        _logger.LogInformation("Created target {TargetId} for user {UserId}", targetId, apiKey);

        // Map the entity to a DTO and return.
        return _mapper.Map<TargetRecordDto>(target);
    }

    [GeneratedRegex("^data:image/[a-zA-Z]+;base64,")]
    private static partial Regex Base64ImageRegex();

    #endregion
}