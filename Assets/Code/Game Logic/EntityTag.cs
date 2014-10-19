using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    public class EntityTag : MonoBehaviour
    {
        private Tag _Tag;

        public Tag TestTagField;

        /// <summary>
        /// Checks to see if this Entity is tagged with a certain EntityTag
        /// </summary>
        /// <param name="Tag">Tag to check against</param>
        /// <returns></returns>
        public bool Is(Tag Tag)
        {
            return (_Tag & Tag) == Tag ? true : false;
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="Tag">Tag to add</param>
        public void AddTag(Tag Tag)
        {
            _Tag |= Tag;
        }

        /// <summary>
        /// Removes a tag
        /// </summary>
        /// <param name="Tag">Tag to remove</param>
        public void RemoveTag(Tag Tag)
        {
            _Tag &= ~Tag;
        }

        /// <summary>
        /// Gets Tag
        /// </summary>
        /// <returns>The Tag</returns>
        public Tag GetEntityTag()
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