using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Lesson entities.
    /// </summary>
    public interface ILessonRepository : IRepository<Lesson>
    {
        /// <summary>
        /// Retrieves all lessons for a specific course, ordered sequentially by SortOrder and CreatedAt.
        /// </summary>
        /// <param name="courseId">The ID of the parent course.</param>
        /// <returns>A Result containing the ordered sequence of lessons.</returns>
        Task<Result<IEnumerable<Lesson>>> GetLessonsByCourseIdAsync(Guid courseId);
    }
}