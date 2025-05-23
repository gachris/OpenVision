﻿using OpenVision.Shared.Contracts;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a filter for pagination.
/// </summary>
public record BrowserQuery : IBrowserQuery
{
    /// <summary>
    /// Gets or sets the number of the page to retrieve.
    /// </summary>
    public virtual int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items to include in a page.
    /// </summary>
    public virtual int Size { get; set; } = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserQuery"/> class.
    /// </summary>
    public BrowserQuery()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserQuery"/> class with the specified page number and page size.
    /// </summary>
    /// <param name="page">The number of the page to retrieve.</param>
    /// <param name="size">The number of items to include in a page.</param>
    public BrowserQuery(int page, int size)
    {
        Page = page < 1 ? 1 : page;
        Size = size < 1 ? 1 : size;
    }
}