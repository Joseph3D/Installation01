using UnityEngine;
using System;
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
        #region Editor Visible Traits
        public float Damage;
        public int MagazineSize;
        public TimeSpan FireInterval;
        public TimeSpan ReloadTime;
        public int RoundsPerTriggerPull;
        public float AccuracyFalloff; // higher values == gets innacurate faster
        public bool SemiAutomatic;
        public string ProjectileAssetName;
        #endregion

        private GameManager Manager;
        private GameObject Projectile;

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

        /// <summary>
        /// Verifies that the selected projectile is properly configured
        /// to be used in engine as a projectile
        /// </summary>
        /// <returns></returns>
        private bool VerifyProjectile()
        {
            if(Manager.ResourceCacheContains(ProjectileAssetName))
            {
                return true;
            }
            return true;
        }
    }
}