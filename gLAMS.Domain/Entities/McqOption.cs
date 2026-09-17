using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;
using gLAMS.Domain.Interfaces;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a selectable answer option for a Multiple Choice Question (MCQ).
    /// Implements ISortable to maintain the exact order of choices (A, B, C, D).
    /// </summary>
    public class McqOption : BaseEntity, ISortable
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent question.
        /// </summary>
        [Required(ErrorMessage = "Option's base question's id is required.")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the text for this specific option.
        /// </summary>
        [Required(ErrorMessage = "Option's text (answer) is required.")]
        [MaxLength(500, ErrorMessage = "Option's text (answer) cannot exceed 500 characters.")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this option is the correct answer.
        /// </summary>
        [Required(ErrorMessage = "Option's correctness flag is required.")]
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the numerical display order of this option.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="McqOption"/> class.
        /// Required by serializers and ORMs.
        /// </summary>
        public McqOption()
        {
            Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="McqOption"/> class.
        /// </summary>
        /// <param name="questionId">The parent question ID.</param>
        /// <param name="text">The answer text.</param>
        /// <param name="isCorrect">True if this is the correct answer.</param>
        /// <param name="sortOrder">The visual display sequence.</param>
        public McqOption(Guid questionId, string text, bool isCorrect, int sortOrder)
        {
            QuestionId = questionId;
            Text = text;
            IsCorrect = isCorrect;
            SortOrder = sortOrder;
        }
    }
}