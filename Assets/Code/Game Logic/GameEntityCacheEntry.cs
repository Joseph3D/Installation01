using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// Represents a cached game entity.
    /// </summary>
    public sealed class GameEntityCacheEntry
    {
        public GameObject Entity { get; private set; }
        public int EntityHash { get; private set; }
        public EntityTag Tag { get; private set; }

        /// <summary>
        /// Creates this cache entry
        /// Automatically gets reference to the EntityTag of the Entity
        /// </summary>
        /// <param name="Entity">GameEntity to cache</param>
        public GameEntityCacheEntry(GameObject Entity)
        {
            if(Entity.tag != "GameEntity")
            {
                Debug.LogError("Attempted to cache a non GameEntity GameObject. Blocked");
                Entity = null;
                Tag = null;
            }

            this.Entity = Entity;

            Tag = Entity.GetComponent<EntityTag>();

            EntityHash = Entity.GetHashCode();
        }
    }
}