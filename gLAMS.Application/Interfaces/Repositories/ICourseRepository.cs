using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Course entities.
    /// </summary>
    public interface ICourseRepository : IRepository<Course>
    {
        /// <summary>
        /// Retrieves all courses assigned to a specific folder.
        /// </summary>
        /// <param name="folderId">The ID of the parent folder.</param>
        /// <returns>A Result containing the collection of courses.</returns>
        Task<Result<IEnumerable<Course>>> GetCoursesByFolderIdAsync(Guid folderId);

        /// <summary>
        /// Searches for active courses by their name or title.
        /// </summary>
        /// <param name="name">The search term to match.</param>
        /// <returns>A Result containing the matching courses.</returns>
        Task<Result<IEnumerable<Course>>> SearchCoursesByNameAsync(string name);
    }
}