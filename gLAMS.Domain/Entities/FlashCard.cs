using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a two-sided flashcard used for spaced repetition and memorization.
    /// </summary>
    public class FlashCard : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent course.
        /// </summary>
        [Required(ErrorMessage = "FlashCard's base course is required.")]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the specific lesson this flashcard belongs to.
        /// Nullable if the flashcard applies to the entire course.
        /// </summary>
        public Guid? LessonId { get; set; }

        /// <summary>
        /// Gets or sets the prompt or question displayed on the front of the card.
        /// </summary>
        [Required(ErrorMessage = "FlashCard's front content is required.")]
        public string FrontContent { get; set; }

        /// <summary>
        /// Gets or sets the answer or details displayed on the back of the card.
        /// </summary>
        [Required(ErrorMessage = "FlashCard's back content is required.")]
        public string BackContent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashCard"/> class.
        /// </summary>
        public FlashCard()
        {
            FrontContent = string.Empty;
            BackContent = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashCard"/> class with required details.
        /// </summary>
        public FlashCard(Guid courseId, string frontContent, string backContent, Guid? lessonId = null)
        {
            CourseId = courseId;
            FrontContent = frontContent;
            BackContent = backContent;
            LessonId = lessonId;
        }
    }
}