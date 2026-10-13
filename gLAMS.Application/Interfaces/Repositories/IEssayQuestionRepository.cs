using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines data access operations specifically for Essay Questions.
    /// </summary>
    public interface IEssayQuestionRepository : IRepository<EssayQuestion>
    {
        /// <summary>
        /// Retrieves all Essay questions assigned to a specific lesson (with their model answer).
        /// </summary>
        Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByLessonIdAsync(Guid lessonId);
        /// <summary>
        /// Retrieves all Essay questions assigned to a course (with their model answer).
        /// </summary>
        Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByCourseIdAsync(Guid courseId);
    }
}
