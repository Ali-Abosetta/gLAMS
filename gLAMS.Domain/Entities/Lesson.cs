using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;
using gLAMS.Domain.Interfaces;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a single learning module or class within a course.
    /// Implements ISortable so instructors can define the exact playback sequence.
    /// </summary>
    public class Lesson : BaseEntity, ISortable
    {
        /// <summary>
        /// Gets or sets the unique identifier of the course this lesson belongs to.
        /// </summary>
        [Required(ErrorMessage = "Lesson's course is required.")]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the title of the lesson.
        /// </summary>
        [Required(ErrorMessage = "Lesson's title is required.")]
        [MaxLength(200, ErrorMessage = "Lesson's title cannot exceed 200 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the numerical order of the lesson within the course.
        /// Used to display lessons in the exact sequence defined by the instructor.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lesson"/> class.
        /// Required by serializers and ORMs.
        /// </summary>
        public Lesson()
        {
            Title = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lesson"/> class with a specific title, course, and sort order.
        /// </summary>
        /// <param name="title">The title of the lesson.</param>
        /// <param name="courseId">The ID of the parent course.</param>
        /// <param name="sortOrder">The visual display order of the lesson.</param>
        public Lesson(string title, Guid courseId, int sortOrder)
        {
            Title = title;
            CourseId = courseId;
            SortOrder = sortOrder;
        }
    }
}