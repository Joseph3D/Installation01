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

    public enum WeaponType
    {
        SemiAutomatic = 0,
        Burst = 1,
        FullyAutomatic = 2,
    }

    public sealed class Weapon : MonoBehaviour
    {
        #region Editor Visible Traits
        public int MagazineCapacity;
        public float FireInterval; // time between shots ( seconds )
        public float BurstFireInterval; // time between rounds in the burst
        public float ReloadInterval; // time it takes to reload ( seconds )
        public int RoundsPerTriggerPull;
        public float AccuracyFalloff; // higher values == gets innacurate faster
        public string ProjectileAssetName;
        public bool BottomlessClip;
        public WeaponType Type;
        #endregion

        private GameManager Manager;
        private Projectile _Projectile;

        private WeaponState State = WeaponState.Idle; 
        private int Magazine = 0; // Weapons magazine
        private bool CanFire = true;

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

        IEnumerator Reload()
        {
            State = WeaponState.Reloading;

            // eventually trigger reload animations somewhere here.

            // and anything else that has to happen to "start" the reload process

            yield return new WaitForSeconds(ReloadInterval); // Wait to complete reload interval ( this is when the entity would be pysically removing and replacing the magazine )

            Magazine = MagazineCapacity; // reset magazine count

            State = WeaponState.Idle;
        }

        IEnumerator Fire(Vector3 Direction)
        {
            switch(Type)
            {
                case WeaponType.SemiAutomatic:
                    {
                        Projectile SpawnedProjectile = GameObject.Instantiate(_Projectile) as Projectile;
                        SpawnedProjectile.SetDirection(Direction);
                        Magazine--;
                        CanFire = false;
                        yield return new WaitForSeconds(FireInterval);
                        CanFire = true;
                        break;
                    }
            }
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