﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetTargetByIdQuery and returns the target DTO if found.
/// </summary>
public class GetTargetByIdQueryHandler : IRequestHandler<GetTargetByIdQuery, TargetDto?>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTargetByIdQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTargetByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">Repository for accessing image targets.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger instance used for logging informational messages and errors.</param>
    public GetTargetByIdQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetTargetByIdQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetTargetByIdQuery request.
    /// Retrieves the image target for the current user based on the target ID,
    /// includes the associated database, and maps the entity to a <see cref="TargetDto"/>.
    /// </summary>
    /// <param name="request">The query request containing the target ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="TargetDto"/> representing the target if found; otherwise, null.
    /// </returns>
    public async Task<TargetDto?> Handle(GetTargetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Getting target {TargetId} for user {UserId}", request.TargetId, userId);

        var imageTargetsQueryable = await _imageTargetsRepository.GetAsync();

        var imageTarget = await imageTargetsQueryable
            .Include(x => x.Database)
            .Where(x => x.Id == request.TargetId && x.Database.UserId == userId)
            .SingleOrDefaultAsync(cancellationToken);

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found for user {UserId}", request.TargetId, userId);
            return null;
        }

        return _mapper.Map<TargetDto>(imageTarget);
    }

    #endregion
}
