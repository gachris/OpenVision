using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a paged response message with data returned by the API.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public class PagedResponse<TResult> : ResponseMessage<TResult>, IPagedResponse<TResult>
{
    /// <summary>
    /// Gets the page number of the current page.
    /// </summary>
    [JsonPropertyName("page")]
    public virtual int Page { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    [JsonPropertyName("size")]
    public virtual int Size { get; }

    /// <summary>
    /// Gets the URI of the first page.
    /// </summary>
    [JsonPropertyName("first_page")]
    public virtual Uri? FirstPage { get; }

    /// <summary>
    /// Gets the URI of the last page.
    /// </summary>
    [JsonPropertyName("last_page")]
    public virtual Uri? LastPage { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    [JsonPropertyName("total_pages")]
    public virtual int TotalPages { get; }

    /// <summary>
    /// Gets the total number of records.
    /// </summary>
    [JsonPropertyName("total_records")]
    public virtual int TotalRecords { get; }

    /// <summary>
    /// Gets the URI of the next page.
    /// </summary>
    [JsonPropertyName("next_page")]
    public virtual Uri? NextPage { get; }

    /// <summary>
    /// Gets the URI of the previous page.
    /// </summary>
    [JsonPropertyName("previous_page")]
    public virtual Uri? PreviousPage { get; }

    /// <summary>
    /// Initializes a new instance of the PagedResponse class with the specified page number, page size, first page URI, last page URI, total number of pages, total number of records, next page URI, and previous page URI.
    /// </summary>
    /// <param name="page">The current page.</param>
    /// <param name="size">The number of items per page.</param>
    /// <param name="firstPage">The URI of the first page.</param>
    /// <param name="lastPage">The URI of the last page.</param>
    /// <param name="totalPages">The total number of pages.</param>
    /// <param name="totalRecords">The total number of records.</param>
    /// <param name="nextPage">The URI of the next page.</param>
    /// <param name="previousPage">The URI of the previous page.</param>
    /// <param name="response">The response payload of type <typeparamref name="TResult"/>.</param>
    /// <param name="transactionId">The unique identifier for the API transaction.</param>
    /// <param name="statusCode">The status code that indicates the success or failure of the API transaction.</param>
    /// <param name="errors">The list of errors associated with the response.</param>
    public PagedResponse(int page,
                         int size,
                         Uri? firstPage,
                         Uri? lastPage,
                         int totalPages,
                         int totalRecords,
                         Uri? nextPage,
                         Uri? previousPage,
                         ResponseDoc<TResult> response,
                         Guid transactionId,
                         StatusCode statusCode,
                         IReadOnlyCollection<Error> errors) : base(response, transactionId, statusCode, errors)
    {
        Page = page;
        Size = size;
        FirstPage = firstPage;
        LastPage = lastPage;
        TotalPages = totalPages;
        TotalRecords = totalRecords;
        NextPage = nextPage;
        PreviousPage = previousPage;
    }
}