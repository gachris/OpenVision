namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a paged response message returned by the API.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
public interface IPagedResponse<TResult> : IResponseMessage<TResult>
{
    /// <summary>
    /// Gets the current page of the response.
    /// </summary>
    int Page { get; }

    /// <summary>
    /// Gets the page size of the response.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Gets the URI for the first page of the response.
    /// </summary>
    Uri? FirstPage { get; }

    /// <summary>
    /// Gets the URI for the last page of the response.
    /// </summary>
    Uri? LastPage { get; }

    /// <summary>
    /// Gets the total number of pages in the response.
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Gets the total number of records in the response.
    /// </summary>
    int TotalRecords { get; }

    /// <summary>
    /// Gets the URI for the next page of the response.
    /// </summary>
    Uri? NextPage { get; }

    /// <summary>
    /// Gets the URI for the previous page of the response.
    /// </summary>
    Uri? PreviousPage { get; }
}