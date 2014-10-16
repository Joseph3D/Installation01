using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    public class EntityTagManager : MonoBehaviour
    {
        private EntityTag _Tag;

        public EntityTag TestTagField;

        /// <summary>
        /// Checks to see if this Entity is tagged with a certain EntityTag
        /// </summary>
        /// <param name="Tag">Tag to check against</param>
        /// <returns></returns>
        public bool Is(EntityTag Tag)
        {
            return (_Tag & Tag) == Tag ? true : false;
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="Tag">Tag to add</param>
        public void AddTag(EntityTag Tag)
        {
            _Tag |= Tag;
        }

        /// <summary>
        /// Removes a tag
        /// </summary>
        /// <param name="Tag">Tag to remove</param>
        public void RemoveTag(EntityTag Tag)
        {
            _Tag &= ~Tag;
        }

        /// <summary>
        /// Gets Tag
        /// </summary>
        /// <returns>The Tag</returns>
        public EntityTag GetEntityTag()
        {
            return _Tag;
        }

        public void Start()
        {

        }
        public void Update()
        {

        }
        public void FixedUpdate()
        {

        }
    }
}