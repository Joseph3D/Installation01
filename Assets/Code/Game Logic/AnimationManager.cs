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
        public string[] AnimationNames;

        private Animator Animator;

        private Dictionary<string,int> AnimationHashes;

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

        private void InitializeInternals()
        {
            Animator = GetComponent<Animator>();

            AnimationHashes = new Dictionary<string, int>();

            CacheAnimationHashes();
        }

        public void PlayAnimation(string Name)
        {
        
        }

        public void PlayAnimation(int AnimationHash)
        {

        }

        private void CacheAnimationHashes()
        {
            if(AnimationNames.Length > 0)
            {
                for(int i = 0; i < AnimationNames.Length; ++i)
                {
                    AnimationHashes.Add(AnimationNames[i], Animator.StringToHash(AnimationNames[i]));
                }
            }
        }
    }
}