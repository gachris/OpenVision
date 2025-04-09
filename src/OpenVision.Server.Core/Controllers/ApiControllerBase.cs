using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Requests;
using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// Provides common functionality for API controllers.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    #region Field/Consts

    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiControllerBase"/> class.
    /// This constructor sets up the essential dependencies required by the controller,
    /// such as the AutoMapper instance and logger.
    /// </summary>
    /// <param name="mapper">
    /// An <see cref="IMapper"/> instance used for mapping between different types,
    /// for example mapping DTOs to domain models or vice versa.
    /// </param>
    /// <param name="logger">
    /// An <see cref="ILogger"/> instance used for logging information, warnings, and errors.
    /// </param>
    protected ApiControllerBase(IMapper mapper, ILogger logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Executes an asynchronous action within a try-catch block.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The resulting IActionResult.</returns>
    protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, Error("An internal server error occurred."));
        }
    }

    /// <summary>
    /// Creates a success response message with no result.
    /// </summary>
    /// <returns>A success response message with no result.</returns>
    protected static IResponseMessage Success()
    {
        return new ResponseMessage(Guid.NewGuid(), Shared.StatusCode.Success, []);
    }

    /// <summary>
    /// Creates an error response message.
    /// </summary>
    /// <returns>An error response message.</returns>
    protected static IResponseMessage Error(string message)
    {
        return new ResponseMessage(Guid.NewGuid(), Shared.StatusCode.Success, [new(ResultCode.InternalServerError, message)]);
    }

    /// <summary>
    /// Creates a success response message with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A success response message with the specified result.</returns>
    protected static IResponseMessage<TResult> Success<TResult>(TResult result)
    {
        return new ResponseMessage<TResult>(new ResponseDoc<TResult>(result), Guid.NewGuid(), Shared.StatusCode.Success, []);
    }

    /// <summary>
    /// Builds a pagination URL using the Url.Action method with the specified pagination filter.
    /// </summary>
    /// <param name="filter">The pagination filter containing the page and size parameters.</param>
    /// <returns>A <see cref="Uri"/> representing the pagination link.</returns>
    private Uri BuildPageUri(BrowserQuery filter)
    {
        var request = HttpContext.Request;
        var url = Url.Action("Get", new { page = filter.Page, size = filter.Size });
        var baseUri = $"{request.Scheme}://{request.Host}{url}";

        if (string.IsNullOrEmpty(baseUri))
        {
            throw new InvalidOperationException("Unable to generate URL for the pagination link.");
        }

        return new Uri(baseUri);
    }

    /// <summary>
    /// Retrieves a paginated response of targets for the query parameters.
    /// </summary>
    /// <param name="query">The pagination and filtering query parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IPagedResponse{T}"/> containing a collection of <see cref="TargetResponse"/> objects.</returns>
    protected async Task<IPagedResponse<IEnumerable<TTarget>>> GetPagedResponseAsync<TSource, TTarget>(
        IQueryable<TSource> queryable,
        BrowserQuery query,
        CancellationToken cancellationToken)
    {
        var validFilter = new BrowserQuery(query.Page, query.Size);
        var take = validFilter.Size;
        var skip = validFilter.Page - 1;

        var totalRecords = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip(skip * take)
            .Take(take)
            .ToListAsync(cancellationToken);

        var totalPages = totalRecords / (double)validFilter.Size;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var nextPage = validFilter.Page >= 1 && validFilter.Page < roundedTotalPages
            ? BuildPageUri(new BrowserQuery(validFilter.Page + 1, validFilter.Size))
            : null;

        var previousPage = validFilter.Page - 1 >= 1 && validFilter.Page <= roundedTotalPages
            ? BuildPageUri(new BrowserQuery(validFilter.Page - 1, validFilter.Size))
            : null;

        var firstPage = BuildPageUri(new BrowserQuery(1, validFilter.Size));
        var lastPage = BuildPageUri(new BrowserQuery(roundedTotalPages, validFilter.Size));

        return new PagedResponse<IEnumerable<TTarget>>(
            validFilter.Page,
            validFilter.Size,
            firstPage,
            lastPage,
            roundedTotalPages,
            totalRecords,
            nextPage,
            previousPage,
            new(_mapper.Map<IEnumerable<TTarget>>(items)),
            Guid.NewGuid(),
            Shared.StatusCode.Success,
            []);
    }

    #endregion
}