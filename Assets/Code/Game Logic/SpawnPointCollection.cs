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
    public sealed class SpawnPointCollection
    {
        private List<GameObject> SpawnPoints;

        public int Count
        {
            get
            {
                return SpawnPoints.Count;
            }
        }

        public SpawnPointCollection()
        {
            SpawnPoints = new List<GameObject>();
        }

        /// <summary>
        /// caches all spawnpoints in game world into this collection
        /// </summary>
        public void CacheAll()
        {
            GameObject[] Entities = GameObject.FindGameObjectsWithTag("GameEntity");
            if (Entities.Length > 0)
            {
                for(int i = 0; i < Entities.Length; ++i)
                {
                    EntityTag TagManager = Entities[i].GetComponent<EntityTag>();

                    if(TagManager.Is(Tag.SpawnPoint))
                    {
                        SpawnPoints.Add(Entities[i]);
                    }
                }
            }
        }
    }
}