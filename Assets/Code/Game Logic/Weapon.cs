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
    public enum WeaponState
    {
        Idle = 0,
        Firing = 1,
        Reloading = 2,
        Jammed_DoubleFeed = 3,
        Jammed_StovePiped = 4,
    }

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
        public bool BottomlessClip;
        #endregion

        private GameManager Manager;
        private Projectile Projectile;
        private WeaponState State = WeaponState.Idle;
        private int Magazine;

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

            if(!VerifyProjectile())
            {
                Debug.LogError("Unable to verify projectile");
            }
        }

        /// <summary>
        /// Verifies that the selected projectile is properly configured
        /// to be used in engine as a projectile
        /// </summary>
        /// <returns>true if the specified projectile prefab has been loaded by the GameManager</returns>
        private bool VerifyProjectile()
        {
            if(Manager.ResourceCacheContains(ProjectileAssetName)) // the specified projectile prefab has already been loaded by the game manager
            {
                Projectile = Manager.GetResourceCacheItemByName(ProjectileAssetName) as Projectile;
                if(Projectile == null)
                {
                    Debug.LogError("Unable to acquire reference to projectile prefab from GameManager");
                    return false;
                }
                return true;
            }
            return false; // for now. later we might want to trigger the GameManager to load this asset, possible asyncronously.
        }
    }
}