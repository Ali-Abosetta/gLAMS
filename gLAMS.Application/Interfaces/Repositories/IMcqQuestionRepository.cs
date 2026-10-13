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
    /// Defines data access operations specifically for Multiple Choice Questions (MCQ).
    /// </summary>
    public interface IMcqQuestionRepository : IRepository<McqQuestion>
    {
        /// <summary>
        /// Retrieves all MCQ questions assigned to a specific lesson (with their options).
        /// </summary>
        Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByLessonIdAsync(Guid lessonId);
        /// <summary>
        /// Retrieves all MCQ questions assigned to a course (with their options).
        /// </summary>
        Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByCourseIdAsync(Guid courseId);
    }
}

