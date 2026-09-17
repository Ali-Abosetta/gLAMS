using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Reference entities.
    /// Contracts for the underlying Stored Procedures.
    /// </summary>
    public interface IReferenceRepository : IRepository<Reference>
    {
        /// <summary>
        /// Retrieves all supplementary references assigned specifically to a given lesson.
        /// </summary>
        /// <param name="lessonId">The unique identifier of the lesson.</param>
        /// <returns>A Result containing the collection of references.</returns>
        Task<Result<IEnumerable<Reference>>> GetReferencesByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Retrieves general supplementary references assigned directly to a course (where LessonId is null).
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A Result containing the collection of course-level references.</returns>
        Task<Result<IEnumerable<Reference>>> GetCourseLevelReferencesAsync(Guid courseId);
    }
}