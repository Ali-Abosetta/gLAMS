using System;
using System.ComponentModel.DataAnnotations;

namespace gLAMS.Domain.Entities.Base
{
    /// <summary>
    /// Represents the foundation for all database entities in the system.
    /// Contains common properties required for tracking, syncing, and soft-deletion.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// Using Guid is the industry standard for offline-first mobile sync engines.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was initially created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was last modified.
        /// Used by the mobile delta-sync engine to find new changes.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the record is soft-deleted.
        /// If true, the record is hidden from the UI but kept in the database to tell phones to delete it.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// Sets the default values for newly created entities.
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}