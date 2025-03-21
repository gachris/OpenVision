using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Dataset;
using OpenVision.Server.Core.Properties;
using OpenVision.Server.Core.Utils;
using OpenVision.Server.EntityFramework.DbContexts;
using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service class that provides functionality to download files.
/// </summary>
public class FilesService : BaseApiService, IFilesService
{
    #region Fields/Consts

    private const string? FileExtension = ".bin";

    private readonly ApplicationDbContext _applicationContext;
    private readonly HttpContext _httpContext;

    #endregion

    /// <summary>
    /// Initializes a new instance of the FilesService class.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="httpContextAccessor">The accessor for the current HTTP context.</param>
    public FilesService(ApplicationDbContext applicationContext,
                        IHttpContextAccessor httpContextAccessor)
    {
        _applicationContext = applicationContext;
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DownloadFileResult>> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = await _applicationContext.Databases.SingleOrDefaultAsync(x => x.Id == id
                                                                                && x.UserId == userId
                                                                                && x.Type == DatabaseType.Device,
                                                                                cancellationToken);

        database.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.DatabaseNotFound);

        var fileDownloadName = Path.ChangeExtension(database.Name, FileExtension);
        var tempFileName = Path.ChangeExtension(Path.GetTempFileName(), FileExtension);

        await _applicationContext.Entry(database).Collection(x => x.ImageTargets).LoadAsync(cancellationToken);

        var targets = database.ImageTargets.Select(imageTarget => new Target(imageTarget.Id.ToString(),
                                                                             imageTarget.AfterProcessImage,
                                                                             imageTarget.Keypoints,
                                                                             imageTarget.Descriptors,
                                                                             imageTarget.DescriptorsRows,
                                                                             imageTarget.DescriptorsCols,
                                                                             imageTarget.Width,
                                                                             imageTarget.Height)).ToList();

        await DatasetSerializer.SerializeAsync(tempFileName, targets);

        var fileContents = await File.ReadAllBytesAsync(tempFileName, cancellationToken);

        File.Delete(tempFileName);

        var response = new DownloadFileResult(fileDownloadName, fileContents, System.Net.Mime.MediaTypeNames.Application.Octet);
        return Success(response);
    }
}