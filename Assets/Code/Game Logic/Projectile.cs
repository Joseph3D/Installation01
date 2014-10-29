using UnityEngine;
using System.Collections;

namespace GameLogic
{
    public class Projectile : MonoBehaviour
    {
        #region Members
        public string TraitsFile;

        public Vector3 DebugDirection;

        public ProjectileTraits Traits;

        private TrailRenderer TracerRenderer;

        private Vector3 Direction;
        private GameManager Manager;

        public Vector3 ProjectileDirection
        {
            get
            {
                return Direction;
            }
        }
        #endregion

        void Awake()
        {

        }

        void Start()
        {
            InitializeTraits();
        }

        void Update()
        {
            Traits.Lifespan--;
            if(Traits.Lifespan == 0)
            {
                RemoveFromWorld();
            }
            else
            {
                UpdateProjectileVelocity();

                UpdateProjectileDrop();
            }
        }

        #region Ballistics Update Functions
        private void UpdateProjectileVelocity()
        {
            Vector3 VelocityVector = Direction * Traits.Velocity;
            transform.position += VelocityVector;
        }

        private void UpdateProjectileDrop()
        {
            Vector3 BulletDropVector = new Vector3(0,Traits.Weight, 0);

            transform.position -= BulletDropVector; // simplistic bullet drop model for now
        }

        private void UpdateProjectileHoming()
        {
            //not yet implemented
        }
        #endregion

        /// <summary>
        /// Sets Direction of this projectile
        /// </summary>
        /// <param name="Direction"></param>
        public void SetDirection(Vector3 Direction)
        {
            this.Direction = Direction;
            this.Direction.Normalize(); // Pre-Normalize
        }

        /// <summary>
        /// Sets the traits of this projectile
        /// </summary>
        /// <param name="Traits"></param>
        public void SetProjectileTraits(ProjectileTraits Traits)
        {
            this.Traits = Traits;
        }

        private void InitializeTraits()
        {
            if(TraitsFile != string.Empty)
            {
                Traits = ProjectileTraits.LoadFromFile(TraitsFile);
            }
        }

        private void RemoveFromWorld()
        {
            Manager.RemoveGameEntityCacheEntry(this.GetHashCode());
            Destroy(this);
        }
    }
}