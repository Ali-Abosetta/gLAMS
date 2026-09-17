using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents a course container that holds learning materials.
    /// Belongs to a specific folder in the hierarchical structure.
    /// </summary>
    public class Course : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the folder this course belongs to.
        /// </summary>
        [Required(ErrorMessage = "Course folder id is required.")]
        public Guid FolderId { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(150, ErrorMessage = "Course name cannot exceed 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// Required by some serializers and ORMs.
        /// </summary>
        public Course()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class with a specific name and folder.
        /// </summary>
        /// <param name="name">The name of the course.</param>
        /// <param name="folderId">The ID of the parent folder.</param>
        public Course(string name, Guid folderId)
        {
            Name = name;
            FolderId = folderId;
        }
    }
}