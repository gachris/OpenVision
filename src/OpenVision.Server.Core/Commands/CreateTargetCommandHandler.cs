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
using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the CreateTargetCommand by processing the image, extracting features,
/// and creating a new target associated with a database.
/// </summary>
public class CreateTargetCommandHandler : IRequestHandler<CreateTargetCommand, TargetDto>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly IDatabaseSpecificationFactory _databaseSpecificationFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTargetCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="databaseSpecificationFactory">The factory used to create the appropriate target specification based on the current authentication context.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational and error messages.</param>
    public CreateTargetCommandHandler(
        IDatabasesRepository databasesRepository,
        IImageTargetsRepository imageTargetsRepository,
        IDatabaseSpecificationFactory databaseSpecificationFactory,
        IMapper mapper,
        ILogger<CreateTargetCommandHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _imageTargetsRepository = imageTargetsRepository;
        _databaseSpecificationFactory = databaseSpecificationFactory;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the CreateTargetCommand request by creating a new target.
    /// </summary>
    /// <param name="request">The command containing the target creation details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="TargetDto"/> representing the newly created target.</returns>
    public async Task<TargetDto> Handle(CreateTargetCommand request, CancellationToken cancellationToken)
    {
        var createDto = request.CreateTargetDto;
        var targetId = Guid.NewGuid();

        var databaseForUserSpecification = createDto.DatabaseId is null 
            ? _databaseSpecificationFactory.GetDatabaseSpecification()
            : _databaseSpecificationFactory.GetDatabaseSpecification(createDto.DatabaseId.Value);
        var databases = await _databasesRepository.GetBySpecificationAsync(databaseForUserSpecification, cancellationToken);
        var database = databases.SingleOrDefault() ?? throw new ArgumentException("Database not found.");

        // Process the image.
        var image = createDto.Image.ToMat();
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
            Type = database.Type is DatabaseType.Cloud ? TargetType.Cloud : createDto.Type,
            Width = createDto.Width,
            Height = height,
            Metadata = createDto.Metadata,
            ActiveFlag = createDto.ActiveFlag ?? ActiveFlag.True,
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

        _logger.LogInformation("Created target {TargetId}", targetId);

        // Map the entity to a DTO and return.
        return _mapper.Map<TargetDto>(target);
    }

    #endregion
}