using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;
using gLAMS.Domain.Enums;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents the abstract foundation for all question types in the gLAMS.
    /// Cannot be instantiated directly.
    /// </summary>
    public abstract class Question : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent course.
        /// </summary>
        [Required(ErrorMessage = "Question's course is required.")]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the lesson this question belongs to.
        /// </summary>
        public Guid? LessonId { get; set; }

        /// <summary>
        /// Gets or sets the type of the question.
        /// Used by Entity Framework as a discriminator to instantiate the correct child class.
        /// </summary>
        [Required(ErrorMessage = "Question's type is required.")]
        public QuestionType Type { get; set; }

        /// <summary>
        /// Gets or sets the actual text or prompt for the question.
        /// </summary>
        [Required(ErrorMessage = "Question's text is required.")]
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// Protected to prevent direct instantiation.
        /// </summary>
        protected Question()
        {
            Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        protected Question(Guid courseId, QuestionType type, string text, Guid? lessonId = null)
        {
            CourseId = courseId;
            Type = type;
            Text = text;
            LessonId = lessonId;
        }
    }
}