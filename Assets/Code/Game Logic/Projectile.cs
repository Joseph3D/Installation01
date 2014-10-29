using UnityEngine;
using System.Collections;

namespace GameLogic
{
    public class Projectile : MonoBehaviour
    {
        #region Members
        #region Editor Visible Traits
        public float Velocity;
        public float Weight;
        public float Lifespan;
        public float Damage;
        public bool Homing;
        public float MaxHomingTurn;
        public bool Tracer;
        #endregion
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
            Lifespan--;
            if(Lifespan == 0)
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
            Vector3 VelocityVector = Direction * Velocity;
            transform.position += VelocityVector;
        }

        private void UpdateProjectileDrop()
        {
            Vector3 BulletDropVector = new Vector3(0,Weight, 0);

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

        private void RemoveFromWorld()
        {
            Manager.RemoveGameEntityCacheEntry(this.GetHashCode());
            Destroy(this);
        }
    }
}