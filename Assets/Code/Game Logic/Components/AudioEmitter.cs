using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;

namespace GameLogic
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour
    {
        private GameManager Manager;

        public void Awake()
        {

        }

        public void Start()
        {

        }

        public void Update()
        {

        }


        private void InitializeInternals()
        {
            Manager = GameManagerLocator.Manager;
        }
    }
}