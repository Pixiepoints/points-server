using System;
using Volo.Abp.Domain.Entities;

namespace PointsServer.Entities
{
    /// <inheritdoc cref="IEntity" />
    [Serializable]
    public abstract class PointsServerEntity<TKey> : Entity, IEntity<TKey>
    {
        /// <inheritdoc/>
        public virtual TKey Id { get; set; }

        protected PointsServerEntity()
        {
        }

        protected PointsServerEntity(TKey id)
        {
            Id = id;
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Id = {Id}";
        }
    }
}