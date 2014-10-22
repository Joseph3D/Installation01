using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;

namespace GameLogic
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class TriggerVolume : MonoBehaviour
    {
        #region Members
        /// <summary>
        /// if this is set this trigger volume will trigger when this entity enters.
        /// </summary>
        public GameObject TriggerObject;

        /// <summary>
        /// If set this trigger volume will trigger when any GameEntity with these tags enters
        /// </summary>
        public Tag[] TriggerTags;

        private GameManager Manager;
        #endregion

        public void Start()
        {
            Manager = GameManagerLocator.Manager;
        }

        public void Update()
        {

        }

        void OnTriggerEnter(Collider Other)
        {

        }
    }
}