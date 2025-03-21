using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace OpenVision.Client.Core.Helpers;

/// <summary>
/// Helper methods for pagination functionality.
/// </summary>
public static class PagerHelpers
{
    /// <summary>
    /// Calculates the total number of pages based on the page size and total item count.
    /// </summary>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="totalCount">Total number of items.</param>
    /// <returns>The total number of pages.</returns>
    public static int GetTotalPages(int pageSize, int totalCount)
    {
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }

    /// <summary>
    /// Checks if the specified page is the active (current) page.
    /// </summary>
    /// <param name="currentPage">The current page number.</param>
    /// <param name="currentIteration">The page number to check.</param>
    /// <returns>True if the page is active; otherwise, false.</returns>
    public static bool IsActivePage(int currentPage, int currentIteration)
    {
        return currentPage == currentIteration;
    }

    /// <summary>
    /// Determines whether to show the right pager button based on the maximum number of pages to render, total pages, and current page.
    /// </summary>
    /// <param name="maxPages">Maximum number of pages to render.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="currentPage">Current active page.</param>
    /// <returns>True if the right pager button should be shown; otherwise, false.</returns>
    public static bool ShowRightPagerButton(int maxPages, int totalPages, int currentPage)
    {
        var maxPageToRender = GetMaxPageToRender(maxPages, totalPages, currentPage);
        return maxPageToRender < totalPages;
    }

    /// <summary>
    /// Determines whether to show the left pager button based on the maximum number of pages to render, total pages, and current page.
    /// </summary>
    /// <param name="maxPages">Maximum number of pages to render.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="currentPage">Current active page.</param>
    /// <returns>True if the left pager button should be shown; otherwise, false.</returns>
    public static bool ShowLeftPagerButton(int maxPages, int totalPages, int currentPage)
    {
        var minPageToRender = GetMinPageToRender(maxPages, totalPages, currentPage);
        return minPageToRender > maxPages;
    }

    /// <summary>
    /// Gets the minimum page number to render based on the maximum number of pages to render, total pages, and current page.
    /// </summary>
    /// <param name="maxPages">Maximum number of pages to render.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="currentPage">Current active page.</param>
    /// <returns>The minimum page number to render.</returns>
    public static int GetMinPageToRender(int maxPages, int totalPages, int currentPage)
    {
        const int defaultPageNumber = 1;
        var currentMaxPages = GetMaxPageToRender(maxPages, totalPages, currentPage);

        if (currentMaxPages == defaultPageNumber)
            return currentMaxPages;

        if (currentMaxPages == totalPages)
            currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);

        var minPageToRender = currentMaxPages - maxPages + defaultPageNumber;

        return minPageToRender;
    }

    /// <summary>
    /// Gets the maximum page number based on the maximum number of pages to render, total pages, and current page.
    /// </summary>
    /// <param name="maxPages">Maximum number of pages to render.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="currentPage">Current active page.</param>
    /// <returns>The maximum page number to render.</returns>
    public static int GetMaxPage(int maxPages, int totalPages, int currentPage)
    {
        var result = (int)Math.Ceiling((double)currentPage / maxPages);
        return result * maxPages;
    }

    /// <summary>
    /// Gets the maximum page number to render based on the maximum number of pages to render, total pages, and current page.
    /// </summary>
    /// <param name="maxPages">Maximum number of pages to render.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="currentPage">Current active page.</param>
    /// <returns>The maximum page number to render.</returns>
    public static int GetMaxPageToRender(int maxPages, int totalPages, int currentPage)
    {
        var currentMaxPages = GetMaxPage(maxPages, totalPages, currentPage);
        return currentMaxPages > totalPages ? totalPages : currentMaxPages;
    }

    /// <summary>
    /// Gets the current page number from the query string.
    /// </summary>
    /// <param name="currentPage">String representation of the current page number.</param>
    /// <returns>The current page number as an integer.</returns>
    public static int GetCurrentPage(string? currentPage)
    {
        const int defaultPageNumber = 1;

        try
        {
            return string.IsNullOrEmpty(currentPage) || !int.TryParse(currentPage, out int parsedPage) || parsedPage <= 0
                ? defaultPageNumber
                : parsedPage;
        }
        catch
        {
            return defaultPageNumber;
        }
    }

    /// <summary>
    /// Gets a query string with the specified page number added or updated.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    /// <param name="page">Page number to include in the query string.</param>
    /// <returns>The updated query string.</returns>
    public static QueryString GetQueryString(HttpContext context, int page)
    {
        const string pageKey = "page";

        var queryString = context.Request.QueryString.Value;
        var queryDictionary = QueryHelpers.ParseQuery(queryString);
        queryDictionary.Remove(pageKey); // Remove existing page key

        // Setup new page key
        var queryBuilder = new QueryBuilder(queryDictionary)
            {
                { pageKey, page.ToString() }
            };

        return queryBuilder.ToQueryString();
    }

    /// <summary>
    /// Constructs a URL with the specified base URL and query string.
    /// </summary>
    /// <param name="baseUrl">Base URL.</param>
    /// <param name="queryString">Query string to append.</param>
    /// <returns>The constructed URL.</returns>
    public static string GetUrl(string baseUrl, QueryString queryString)
    {
        return baseUrl + queryString;
    }
}