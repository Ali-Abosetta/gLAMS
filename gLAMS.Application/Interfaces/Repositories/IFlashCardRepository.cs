using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for FlashCard entities.
    /// Contracts for the underlying Stored Procedures.
    /// </summary>
    public interface IFlashCardRepository : IRepository<FlashCard>
    {
        /// <summary>
        /// Retrieves all flashcards assigned specifically to a given lesson.
        /// </summary>
        /// <param name="lessonId">The unique identifier of the lesson.</param>
        /// <returns>A Result containing the collection of flashcards.</returns>
        Task<Result<IEnumerable<FlashCard>>> GetFlashCardsByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Retrieves general flashcards assigned directly to a course (where LessonId is null).
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A Result containing the collection of course-level flashcards.</returns>
        Task<Result<IEnumerable<FlashCard>>> GetCourseLevelFlashCardsAsync(Guid courseId);
    }
}