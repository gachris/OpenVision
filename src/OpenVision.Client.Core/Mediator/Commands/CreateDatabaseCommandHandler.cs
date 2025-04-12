using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Handles the creation of a new database by processing a <see cref="CreateDatabaseCommand"/>.
/// </summary>
public class CreateDatabaseCommandHandler : IRequestHandler<CreateDatabaseCommand, ResultDto<DatabaseResponse>>
{
    #region Fields

    private readonly IDatabaseApiService _databaseApiService;
    private readonly ILogger<CreateDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databaseApiService">The database API service used to create the database.</param>
    /// <param name="logger">The logger for logging diagnostic messages.</param>
    public CreateDatabaseCommandHandler(IDatabaseApiService databaseApiService, ILogger<CreateDatabaseCommandHandler> logger)
    {
        _databaseApiService = databaseApiService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the execution of the <see cref="CreateDatabaseCommand"/> by creating a new database.
    /// </summary>
    /// <param name="request">
    /// The command containing the necessary details to create a new database.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ResultDto{DatabaseResponse}"/> containing the new database details on success, 
    /// or error information if the operation failed.
    /// </returns>
    public async Task<ResultDto<DatabaseResponse>> Handle(CreateDatabaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating database with Name: {Name}", request.Request.Name);
            var response = await _databaseApiService.CreateAsync(request.Request, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to create database: {Error}", error);
                return new ResultDto<DatabaseResponse>(default!, error);
            }

            _logger.LogInformation("Successfully created database with ID: {Id}", response.Response.Result.Id);
            return new ResultDto<DatabaseResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating database.");
            return new ResultDto<DatabaseResponse>(default!, "An unexpected error occurred.", ex);
        }
    }

    #endregion
}