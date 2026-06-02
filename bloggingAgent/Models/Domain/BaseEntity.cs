using System;

namespace BloggingAgent.Models.Domain
{
    /// <summary>
    /// Base entity class with GUID primary key and soft delete support
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier (GUID)
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Creation timestamp in UTC
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last modification timestamp in UTC
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Soft delete flag - when true, entity is considered deleted but retained in database
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Soft delete timestamp in UTC
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Mark entity as deleted (soft delete)
        /// </summary>
        public virtual void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restore a soft-deleted entity
        /// </summary>
        public virtual void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
        }

        /// <summary>
        /// Mark entity as modified
        /// </summary>
        public virtual void MarkAsModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
