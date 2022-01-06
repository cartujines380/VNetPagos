using System;

namespace VisaNet.Domain.Entities.Base.Base
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    public abstract class EntityBase
    {
        #region Properties

        /// <summary>
        /// Get the persisten object identifier.
        /// </summary>
        public virtual Guid Id { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment.
        /// </summary>
        /// <returns>True if entity is transient, else false.</returns>
        public bool IsTransient()
        {
            return Id == Guid.Empty;
        }

        /// <summary>
        /// Generate identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if (IsTransient()) Id = Guid.NewGuid();
        }

        #endregion
    }
}