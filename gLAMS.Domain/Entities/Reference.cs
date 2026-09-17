using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;
using gLAMS.Domain.Enums;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a supplementary resource (File, URL, or Text) attached to a course or specific lesson.
    /// </summary>
    public class Reference : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent course.
        /// </summary>
        [Required(ErrorMessage = "Reference's base course is required.")]
        public Guid CourseId { get; set; } 

        /// <summary>
        /// Gets or sets the unique identifier of the specific lesson this reference belongs to.
        /// Nullable if the reference is for the entire course.
        /// </summary>
        public Guid? LessonId { get; set; }

        /// <summary>
        /// Gets or sets the display title of the reference.
        /// </summary>
        [Required(ErrorMessage = "Reference's title is required.")]
        [MaxLength(200, ErrorMessage = "Reference's title cannot exceed 200 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the format of the reference (e.g., File, Url).
        /// </summary>
        [Required(ErrorMessage = "Reference's type is required.")]
        public ReferenceType Type { get; set; }

        /// <summary>
        /// Gets or sets the actual path, URL, or citation text based on the ReferenceType.
        /// </summary>
        [Required(ErrorMessage = "Reference's content value is required.")]
        public string ContentValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        public Reference()
        {
            Title = string.Empty;
            ContentValue = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class with required details.
        /// </summary>
        public Reference(Guid courseId, string title, ReferenceType type, string contentValue, Guid? lessonId = null)
        {
            CourseId = courseId;
            Title = title;
            Type = type;
            ContentValue = contentValue;
            LessonId = lessonId;
        }
    }
}