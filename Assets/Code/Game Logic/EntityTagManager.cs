using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    public class EntityTagManager : MonoBehaviour
    {
        public string TagFile;

        private EntityTag _Tag;

        /// <summary>
        /// Checks to see if this Entity is tagged with a certain EntityTag
        /// </summary>
        /// <param name="Tag">Tag to check against</param>
        /// <returns></returns>
        public bool Is(EntityTag Tag)
        {
            return (_Tag & Tag) == Tag ? true : false;
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