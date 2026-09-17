using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities.Base;

namespace gLAMS.Domain.Entities
{
    /// <summary>
    /// Represents an organizational container used to group courses or other folders together.
    /// Supports infinite nesting via the ParentId property.
    /// </summary>
    public class Folder : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the folder.
        /// Supports bilingual text (Arabic/English) automatically via the UI and database configuration.
        /// </summary>
        [Required(ErrorMessage = "Folder name is required.")]
        [MaxLength(150, ErrorMessage = "Folder name cannot exceed 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent folder.
        /// If null, this folder is a root-level folder on the main screen.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// Required by some serializers and ORMs.
        /// </summary>
        public Folder()
        {
            // The BaseEntity constructor automatically handles Id, CreatedAt, UpdatedAt, and IsDeleted.
            Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class with a specific name and optional parent.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <param name="parentId">The ID of the parent folder, or null if it is a root folder.</param>
        public Folder(string name, Guid? parentId = null)
        {
            Name = name;
            ParentId = parentId;
        }
    }
}