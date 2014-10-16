using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using GameLogic;

using CodeTimeProfiler = Helpers.Profiler;


namespace Assets.Code.Game_Logic
{
    public class SpawnPoint : MonoBehaviour
    {
        public int AreaCheckRadius;

        public bool ProfileEnabled;

        public int EntitiesNearby { get; private set; }
        public bool PlayerNearby { get; private set; }
        public int EnemyEntitiesNearby { get; private set; }



        public void Start()
        {
            UpdateSurroundingAreaInformation();
        }

        public void Update()
        {

        }

        private void UpdateSurroundingAreaInformation()
        {
#if DEBUG
            if(ProfileEnabled)
                CodeTimeProfiler.BeginPerformanceTimer();
#endif
            RaycastHit[] Hits = Physics.SphereCastAll(transform.position, AreaCheckRadius, Vector3.forward, 0.001f);

            int nearbyEntities = 0;
            bool playerNearby = false;
            int enemyEntitiesNearby = 0;

            if(Hits.Length > 0)
            {
                for(int i = 0; i < Hits.Length; ++i)
                {
                    GameObject HitObject = Hits[i].collider.gameObject;

                    if(HitObject.tag == "GameEntity")
                    {
                        nearbyEntities++;
                        EntityTagManager TagManager = GetComponent<EntityTagManager>() as EntityTagManager;

                        if (TagManager.Is(EntityTag.Enemy))
                            enemyEntitiesNearby++;

                        if (TagManager.Is(EntityTag.Player))
                            playerNearby = true;
                    }
                }

                PlayerNearby = playerNearby;
                EnemyEntitiesNearby = enemyEntitiesNearby;
            }
#if DEBUG
            if (ProfileEnabled)
            {
                CodeTimeProfiler.EndPerformanceTimer();
                long ElapsedMilliseconds = CodeTimeProfiler.GetElapsedMilliseconds(true);
                long ElapsedTicks = CodeTimeProfiler.GetElapsedTicks(true);
                Debug.Log("SpawnPoint.UpdateSurroundingAreaInformation() took " + ElapsedMilliseconds.ToString() + " milliseconds.");
                Debug.Log("SpawnPoint.UpdateSurroundingAreaInformation() took " + ElapsedTicks.ToString() + " ticks.");
            }
#endif
        }
    }
}