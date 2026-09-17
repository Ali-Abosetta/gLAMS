using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Enums;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents an essay-style question requiring a long-form text answer.
    /// Inherits base properties from Question.
    /// </summary>
    public class EssayQuestion : Question
    {
        /// <summary>
        /// Gets or sets the model answer used for grading.
        /// </summary>
        [Required(ErrorMessage = "Essay question's model answer is required.")]
        public string ModelAnswer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EssayQuestion"/> class.
        /// </summary>
        public EssayQuestion()
        {
            ModelAnswer = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EssayQuestion"/> class.
        /// Passes base parameters to the underlying Question class.
        /// </summary>
        public EssayQuestion(Guid courseId, string text, string modelAnswer, Guid? lessonId = null)
            : base(courseId, QuestionType.Essay, text, lessonId)
        {
            ModelAnswer = modelAnswer;
        }
    }
}