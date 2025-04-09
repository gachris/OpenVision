namespace OpenVision.Server.Core.Repositories.Specifications;

/// <summary>
/// Defines a contract for applying specifications to an <see cref="IQueryable{T}"/>.
/// Specifications encapsulate filtering and including related entities,
/// allowing for reusable and modular query criteria.
/// </summary>
/// <typeparam name="T">The type of entity the specification applies to.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Applies the specification's filtering and include criteria to the given query.
    /// </summary>
    /// <param name="query">The query to which the specification is applied.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> with the specification's criteria applied.
    /// </returns>
    IQueryable<T> Apply(IQueryable<T> query);
}