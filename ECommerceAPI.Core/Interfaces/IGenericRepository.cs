using System.Linq.Expressions;

namespace ECommerceAPI.Core.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type this repository works with</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities of type T
    /// </summary>
    /// <returns>A collection of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The added entity</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Removes an entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    Task DeleteAsync(T entity);
}
