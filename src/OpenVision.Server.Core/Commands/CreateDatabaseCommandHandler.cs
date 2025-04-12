using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the CreateDatabaseCommand and creates a new database.
/// </summary>
public class CreateDatabaseCommandHandler : IRequestHandler<CreateDatabaseCommand, DatabaseDto>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly IApiKeysRepository _apiKeysRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApiKeyGeneratorService _apiKeyGeneratorService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="apiKeysRepository">The repository for accessing API keys.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="apiKeyGeneratorService">The service used to generate API keys.</param>
    /// <param name="mapper">The AutoMapper instance for mapping between entities and DTOs.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public CreateDatabaseCommandHandler(
        IDatabasesRepository databasesRepository,
        IApiKeysRepository apiKeysRepository,
        ICurrentUserService currentUserService,
        IApiKeyGeneratorService apiKeyGeneratorService,
        IMapper mapper,
        ILogger<CreateDatabaseCommandHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _apiKeysRepository = apiKeysRepository;
        _currentUserService = currentUserService;
        _apiKeyGeneratorService = apiKeyGeneratorService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the CreateDatabaseCommand request to create a new database.
    /// </summary>
    /// <param name="request">The command containing database creation details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="DatabaseDto"/> representing the newly created database.</returns>
    public async Task<DatabaseDto> Handle(CreateDatabaseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ArgumentException("User identifier not found.");

        var createDatabaseDto = request.CreateDatabaseDto;
        var databaseId = Guid.NewGuid();
        var clientApiKeyId = Guid.NewGuid();
        var serverApiKeyId = Guid.NewGuid();

        _logger.LogInformation("Creating database for user {UserId}", userId);

        var currentTime = DateTimeOffset.Now;

        var database = new Database
        {
            Id = databaseId,
            UserId = userId,
            Name = createDatabaseDto.Name,
            Type = createDatabaseDto.Type,
            Created = currentTime,
            Updated = currentTime,
        };

        var clientApiKeyValue = _apiKeyGeneratorService.GenerateKey();
        var serverApiKeyValue = _apiKeyGeneratorService.GenerateKey();

        var clientApiKey = new ApiKey
        {
            Id = clientApiKeyId,
            Type = ApiKeyType.Client,
            Key = clientApiKeyValue,
            DatabaseId = databaseId,
            Created = currentTime,
            Updated = currentTime,
            Database = database
        };

        var serverApiKey = new ApiKey
        {
            Id = serverApiKeyId,
            Type = ApiKeyType.Server,
            Key = serverApiKeyValue,
            DatabaseId = databaseId,
            Created = currentTime,
            Updated = currentTime,
            Database = database
        };

        database.ApiKeys.Add(clientApiKey);
        database.ApiKeys.Add(serverApiKey);

        await _databasesRepository.CreateAsync(database, cancellationToken);

        _logger.LogInformation("Created database {DatabaseId} for user {UserId}", databaseId, userId);

        return _mapper.Map<DatabaseDto>(database);
    }

    #endregion
}