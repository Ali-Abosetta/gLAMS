using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Enums;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a Multiple Choice Question (MCQ).
    /// Contains a collection of specific answer options.
    /// </summary>
    public class McqQuestion : Question
    {
        /// <summary>
        /// Gets or sets the list of available choices for this question.
        /// </summary>
        [Required(ErrorMessage = "MCQ question options list cannot be null.")]
        public List<McqOption> Options { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="McqQuestion"/> class.
        /// </summary>
        public McqQuestion()
        {
            Options = new List<McqOption>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="McqQuestion"/> class.
        /// Passes base parameters to the underlying Question class.
        /// </summary>
        public McqQuestion(Guid courseId, string text, Guid? lessonId = null)
            : base(courseId, QuestionType.MCQ, text, lessonId)
        {
            Options = new List<McqOption>();
        }
    }
}
