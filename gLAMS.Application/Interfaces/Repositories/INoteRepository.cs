using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Note entities.
    /// Contracts for the underlying Stored Procedures.
    /// </summary>
    public interface INoteRepository : IRepository<Note>
    {
        /// <summary>
        /// Retrieves all notes assigned specifically to a given lesson.
        /// </summary>
        /// <param name="lessonId">The unique identifier of the lesson.</param>
        /// <returns>A Result containing the collection of notes.</returns>
        Task<Result<IEnumerable<Note>>> GetNotesByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Retrieves general notes assigned directly to a course (where LessonId is null).
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A Result containing the collection of course-level notes.</returns>
        Task<Result<IEnumerable<Note>>> GetCourseLevelNotesAsync(Guid courseId);
    }
}