using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Core.Dataset;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;
using OpenVision.Shared;
using Path = System.IO.Path;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the GetDatabaseFileCommand by retrieving the specified database,
/// serializing its targets, and returning a file download result.
/// </summary>
public class GetDatabaseFileCommandHandler : IRequestHandler<GetDatabaseFileCommand, DatabaseFileDto>
{
    #region Fields/Consts

    private const string FileExtension = ".bin";
    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetDatabaseFileCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDatabaseFileCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public GetDatabaseFileCommandHandler(
        IDatabasesRepository databasesRepository,
        ICurrentUserService currentUserService,
        ILogger<GetDatabaseFileCommandHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetDatabaseFileCommand by retrieving the database, serializing its targets,
    /// and returning a file download result.
    /// </summary>
    /// <param name="request">The command request containing the database ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="DatabaseFileDto"/> containing the filename, file contents, and content type.</returns>
    public async Task<DatabaseFileDto> Handle(GetDatabaseFileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("User {UserId} requested download for database {DatabaseId}", userId, request.DatabaseId);

        var databaseForUserSpecification = new DatabaseForUserSpecification(request.DatabaseId, userId)
        {
            Includes =
            {
                database => database.ImageTargets,
                database => database.ApiKeys
            }
        };
        var databases = await _databasesRepository.GetBySpecificationAsync(databaseForUserSpecification, cancellationToken);
        var database = databases.SingleOrDefault();

        if (database is null)
        {
            _logger.LogWarning("Database {DatabaseId} not found for user {UserId}", request.DatabaseId, userId);
            throw new ArgumentNullException(nameof(database));
        }

        if (database.Type is not DatabaseType.Device)
        {
            _logger.LogWarning("Database {DatabaseId} not is not supported for download.", request.DatabaseId);
            throw new ArgumentNullException(nameof(database));
        }

        var fileDownloadName = Path.ChangeExtension(database.Name, FileExtension);
        var tempFileName = Path.ChangeExtension(Path.GetTempFileName(), FileExtension);

        var targets = database.ImageTargets.Select(imageTarget => new Target(
            imageTarget.Id.ToString(),
            imageTarget.AfterProcessImage,
            imageTarget.Keypoints,
            imageTarget.Descriptors,
            imageTarget.DescriptorsRows,
            imageTarget.DescriptorsCols,
            imageTarget.Width,
            imageTarget.Height)).ToList();

        await DatasetSerializer.SerializeAsync(tempFileName, targets, cancellationToken);

        var fileContents = await File.ReadAllBytesAsync(tempFileName, cancellationToken);

        File.Delete(tempFileName);

        _logger.LogInformation("Successfully generated file {FileDownloadName} for database {DatabaseId} and user {UserId}",
            fileDownloadName, request.DatabaseId, userId);

        return new DatabaseFileDto
        {
            Filename = fileDownloadName,
            FileContents = fileContents,
            ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
        };
    }

    #endregion
}
