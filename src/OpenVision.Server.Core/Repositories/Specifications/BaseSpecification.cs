using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// Provides a base implementation for creating specifications that can be applied to queries.
/// This class supports filtering via <see cref="Criteria"/> and including related entities using lambda expressions.
/// </summary>
/// <typeparam name="T">The entity type for which the specification is defined.</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    /// <summary>
    /// Gets the filter criteria to be applied to the query.
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    /// <summary>
    /// Gets the list of lambda expressions used to include related entities in the query.
    /// </summary>
    public List<Expression<Func<T, object?>>> Includes { get; } = new List<Expression<Func<T, object?>>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class without any initial criteria.
    /// </summary>
    protected BaseSpecification() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class with the specified filter criteria.
    /// </summary>
    /// <param name="criteria">A lambda expression representing the filter criteria to apply to the query.</param>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Applies the specification's filter criteria and include expressions to the given query.
    /// </summary>
    /// <param name="query">The query to which the specification is applied.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that has been filtered and configured with the specified include expressions.
    /// </returns>
    public virtual IQueryable<T> Apply(IQueryable<T> query)
    {
        // Apply the filtering criteria if it exists.
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }

        // Apply each include expression to the query.
        query = Includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
