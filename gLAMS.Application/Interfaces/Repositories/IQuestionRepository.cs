using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Question entities.
    /// </summary>
    public interface IQuestionRepository : IRepository<Question>
    {
        /// <summary>
        /// Retrieves all questions assigned to a specific lesson.
        /// </summary>
        /// <param name="lessonId">The ID of the specific lesson.</param>
        /// <returns>A Result containing the collection of questions.</returns>
        Task<Result<IEnumerable<Question>>> GetQuestionsByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Retrieves general questions assigned directly to a course (LessonId is null).
        /// </summary>
        /// <param name="courseId">The ID of the parent course.</param>
        /// <returns>A Result containing the collection of course-level questions.</returns>
        Task<Result<IEnumerable<Question>>> GetCourseLevelQuestionsAsync(Guid courseId);
    }
}