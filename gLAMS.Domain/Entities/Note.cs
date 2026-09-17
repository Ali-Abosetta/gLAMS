using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a text-based note created by the instructor for a course or specific lesson.
    /// </summary>
    public class Note : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the parent course.
        /// </summary>
        [Required(ErrorMessage = "Note's base course is required.")]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the specific lesson this note belongs to.
        /// Nullable if the note applies to the entire course.
        /// </summary>
        public Guid? LessonId { get; set; }

        /// <summary>
        /// Gets or sets the title or subject of the note.
        /// </summary>
        [Required(ErrorMessage = "Note's title is required.")]
        [MaxLength(200, ErrorMessage = "Note's title cannot exceed 200 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the rich text or plain text content of the note.
        /// </summary>
        [Required(ErrorMessage = "Note's text is required.")]
        public string NoteText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        public Note()
        {
            Title = string.Empty;
            NoteText = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class with required details.
        /// </summary>
        public Note(Guid courseId, string title, string noteText, Guid? lessonId = null)
        {
            CourseId = courseId;
            Title = title;
            NoteText = noteText;
            LessonId = lessonId;
        }
    }
}