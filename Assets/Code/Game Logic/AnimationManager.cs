using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;
namespace GameLogic
{
    public class AnimationManager : MonoBehaviour
    {
        private Animator Animator;

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
            Animator = GetComponent<Animator>();
        }
    }
}