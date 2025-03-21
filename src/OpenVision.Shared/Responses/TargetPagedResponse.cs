namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a paged response containing a collection of <see cref="TargetResponse"/> objects.
/// </summary>
public class TargetPagedResponse : PagedResponse<IEnumerable<TargetResponse>>, IPagedResponse<IEnumerable<TargetResponse>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetPagedResponse"/> class with the specified page number, page size, first page URI, last page URI, total number of pages, total number of records, next page URI, and previous page URI.
    /// </summary>
    /// <param name="page">The current page.</param>
    /// <param name="size">The number of items per page.</param>
    /// <param name="firstPage">The URI of the first page.</param>
    /// <param name="lastPage">The URI of the last page.</param>
    /// <param name="totalPages">The total number of pages.</param>
    /// <param name="totalRecords">The total number of records.</param>
    /// <param name="nextPage">The URI of the next page.</param>
    /// <param name="previousPage">The URI of the previous page.</param>
    /// <param name="response">The response payload collection of type <see cref="TargetResponse"/>.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public TargetPagedResponse(int page,
                               int size,
                               Uri? firstPage,
                               Uri? lastPage,
                               int totalPages,
                               int totalRecords,
                               Uri? nextPage,
                               Uri? previousPage,
                               ResponseDoc<IEnumerable<TargetResponse>> response,
                               Guid transactionId,
                               StatusCode statusCode,
                               IReadOnlyCollection<Error> errors) : base(page, size, firstPage, lastPage, totalPages, totalRecords, nextPage, previousPage, response, transactionId, statusCode, errors)
    {
    }
}
