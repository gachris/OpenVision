using System.Data;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Properties;
using OpenVision.Server.Core.Utils;
using OpenVision.Server.EntityFramework.DbContexts;
using OpenVision.Server.EntityFramework.Entities;
using OpenVision.Shared;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;
using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for interacting with databases.
/// </summary>
public class DatabasesService : BaseApiService, IDatabasesService
{
    #region Fields/Consts

    private readonly ApplicationDbContext _applicationContext;
    private readonly HttpContext _httpContext;
    private readonly IApiKeyGeneratorService _apiKeyGeneratorService;
    private readonly IUriService _uriService;
    private readonly IMapper _mapper;
    private readonly ILogger<DatabasesService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesService"/> class.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="apiKeyGeneratorService">The service used to generate API keys.</param>
    /// <param name="uriService">The service used to create page URIs.</param>
    /// <param name="mapper">The mapper service.</param>
    /// <param name="logger">The logger.</param>
    public DatabasesService(ApplicationDbContext applicationContext,
                            IHttpContextAccessor httpContextAccessor,
                            IApiKeyGeneratorService apiKeyGeneratorService,
                            IUriService uriService,
                            IMapper mapper,
                            ILogger<DatabasesService> logger)
    {
        _applicationContext = applicationContext;
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _apiKeyGeneratorService = apiKeyGeneratorService;
        _uriService = uriService;
        _mapper = mapper;
        _logger = logger;
    }

    #region IDatabasesService Implementation

    /// <inheritdoc/>
    public async Task<DatabasePagedResponse> GetAsync(DatabaseBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var route = _httpContext.Request.Path.Value;
        route.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.RouteNotFound);

        var totalRecords = await _applicationContext.Databases.CountAsync(cancellationToken);

        var validFilter = new PaginationFilter(query.Page, query.Size == -1 ? totalRecords : query.Size);
        var take = validFilter.Size;
        var skip = validFilter.Page - 1;

        var databases = await _applicationContext.Databases.Where(x => x.UserId == userId && (string.IsNullOrEmpty(query.SearchText) || x.Name.Contains(query.SearchText)))
                                                           .Include(a => a.ImageTargets)
                                                           .Include(a => a.ApiKeys)
                                                           .OrderBy(x => x.Created)
                                                           .Skip(skip * take)
                                                           .Take(take)
                                                           .ToListAsync(cancellationToken);

        var result = databases.Select(_mapper.Map<DatabaseResponse>);
        var totalPages = totalRecords / (double)validFilter.Size;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var nextPage =
          validFilter.Page >= 1 && validFilter.Page < roundedTotalPages
          ? _uriService.GetPageUri(new PaginationFilter(validFilter.Page + 1, validFilter.Size), route)
          : null;
        var previousPage =
            validFilter.Page - 1 >= 1 && validFilter.Page <= roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter(validFilter.Page - 1, validFilter.Size), route)
            : null;
        var firstPage = _uriService.GetPageUri(new PaginationFilter(1, validFilter.Size), route);
        var lastPage = _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.Size), route);

        var respose = new DatabasePagedResponse(totalPages == 0 ? -1 : validFilter.Page,
                                                totalRecords == 0 ? -1 : validFilter.Size,
                                                totalPages == 0 ? null : firstPage,
                                                totalRecords == 0 ? null : lastPage,
                                                roundedTotalPages,
                                                totalRecords,
                                                nextPage,
                                                previousPage,
                                                new ResponseDoc<IEnumerable<DatabaseResponse>>(result),
                                                Guid.NewGuid(),
                                                StatusCode.Success,
                                                []);

        return respose;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = await _applicationContext.Databases.
            SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        database.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.DatabaseNotFound);

        await _applicationContext.Entry(database)
                                 .Collection(x => x.ImageTargets)
                                 .LoadAsync(cancellationToken);

        await _applicationContext.Entry(database)
                                 .Collection(x => x.ApiKeys)
                                 .LoadAsync(cancellationToken);

        var databaseResponse = _mapper.Map<DatabaseResponse>(database);
        return Success(databaseResponse);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<Guid>> CreateAsync(PostDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        var databaseId = Guid.NewGuid();
        var clientApiKeyId = Guid.NewGuid();
        var serverApiKeyId = Guid.NewGuid();

        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = new Database
        {
            Id = databaseId,
            UserId = userId,
            Name = body.Name!,
            Type = body.Type!.Value,
            Created = DateTimeOffset.Now,
            Updated = DateTimeOffset.Now,
        };

        var clientApiKeyValue = await _apiKeyGeneratorService.GenerateAsync();
        var serverApiKeyValue = await _apiKeyGeneratorService.GenerateAsync();

        var clientApiKey = new ApiKey
        {
            Id = clientApiKeyId,
            Type = ApiKeyType.Client,
            Key = clientApiKeyValue,
            DatabaseId = databaseId,
            Created = DateTimeOffset.Now,
            Updated = DateTimeOffset.Now,
            Database = database
        };

        var serverApiKey = new ApiKey
        {
            Id = serverApiKeyId,
            Type = ApiKeyType.Server,
            Key = serverApiKeyValue,
            DatabaseId = databaseId,
            Created = DateTimeOffset.Now,
            Updated = DateTimeOffset.Now,
            Database = database
        };

        await _applicationContext.ApiKeys.AddAsync(clientApiKey, cancellationToken);
        await _applicationContext.ApiKeys.AddAsync(serverApiKey, cancellationToken);
        await _applicationContext.Databases.AddAsync(database, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success(databaseId);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> UpdateAsync(Guid id, UpdateDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = await _applicationContext.Databases.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        database.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.DatabaseNotFound);

        database.Name = body.Name ?? database.Name;

        database.Updated = DateTimeOffset.Now;

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = await _applicationContext.Databases.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        database.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.DatabaseNotFound);

        await _applicationContext.Entry(database).Collection(x => x.ImageTargets).LoadAsync(cancellationToken);
        await _applicationContext.Entry(database).Collection(x => x.ApiKeys).LoadAsync(cancellationToken);

        _applicationContext.ApiKeys.RemoveRange(database.ApiKeys);
        _applicationContext.ImageTargets.RemoveRange(database.ImageTargets);
        _applicationContext.Databases.Remove(database);

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    #endregion
}
