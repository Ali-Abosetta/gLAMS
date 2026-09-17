using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gLAMS.Domain.Entities.Base;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines the standard CRUD operations for all entities.
    /// Utilizes the Result pattern to ensure safe execution.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Retrieves a specific entity by its unique identifier.
        /// </summary>
        /// <param name="id">The Guid of the entity to retrieve.</param>
        /// <returns>A Result containing the entity if found, or an error message if not.</returns>
        Task<Result<T>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all active entities of this type from the database.
        /// </summary>
        /// <returns>A Result containing a collection of entities.</returns>
        Task<Result<IEnumerable<T>>> GetAllAsync();

        /// <summary>
        /// Inserts a new entity into the database.
        /// </summary>
        /// <param name="entity">The entity instance to insert.</param>
        /// <returns>A Result containing the successfully inserted entity.</returns>
        Task<Result<T>> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity instance with updated values.</param>
        /// <returns>A Result containing the successfully updated entity.</returns>
        Task<Result<T>> UpdateAsync(T entity);

        /// <summary>
        /// Performs a soft deletion of an entity by setting its IsDeleted flag to true.
        /// </summary>
        /// <param name="id">The Guid of the entity to softly delete.</param>
        /// <returns>A Result indicating true if the deletion succeeded.</returns>
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}