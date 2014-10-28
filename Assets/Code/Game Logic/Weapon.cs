using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;

using UnityDebug = UnityEngine.Debug;
using UnityRandom = UnityEngine.Random;

namespace GameLogic
{
    public sealed class Weapon : MonoBehaviour
    {
        private GameManager Manager;

        public void Awake()
        {
            InitializeInternals();
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

        private void InitializeInternals()
        {
            Manager = GameManagerLocator.Manager;
        }
    }
}