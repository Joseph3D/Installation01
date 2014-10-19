using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;

using UnityDebug = UnityEngine.Debug;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        public GameMode InitialMode;
        private GameMode Mode;

        private bool _AssetsLoaded;
        
        public string[] StartupAssets;
        private Dictionary<string, object> ResourceCache;

        public bool AssetsLoaded
        {
            get { return _AssetsLoaded; }
            set { _AssetsLoaded = value; }
        }
        public bool InternalsInitialized
        {
            get
            {
                return _InternalsInitialized;
            }
        }
        private bool _InternalsInitialized;

        private List<GameEntityCacheEntry> GameEntityCache;


        void Start()
        {
            InitializeInternals();
        }

        void Update()
        {
            #region Content Load Check
            if (!_AssetsLoaded)
                LoadAssets();
            #endregion

        }

        private void UpdateState()
        {

        }

        /// <summary>
        /// Initializes all objects/resources that GameManager needs to use
        /// </summary>
        private void InitializeInternals()
        {
            ResourceCache = new Dictionary<string, object>();
            GameEntityCache = new List<GameEntityCacheEntry>();


            _InternalsInitialized = true;
        }

        /// <summary>
        /// Loads all assets that are critical and required before gameplay can start
        /// </summary>
        private void LoadAssets()
        {
            if(StartupAssets.Length > 0)
            {
                for(int i = 0; i < StartupAssets.Length; ++i)
                {
                    LoadGameObject(StartupAssets[i], StartupAssets[i]);
                }
            }
            _AssetsLoaded = true;
        }

        public void AddObjectToCache(string ObjectHandleName, object Handle)
        {
            ResourceCache.Add(ObjectHandleName, Handle);
        }
        public void LoadGameObject(string GameObjectFile,string GameObjectHandle)
        {
            if(ResourceCache.ContainsKey(GameObjectHandle))
            {
                UnityDebug.Log("GameManager is attempting to load: " + GameObjectHandle + " Twice. Blocked.");
                return;
            }
            GameObject LoadedObject = Resources.Load(GameObjectFile) as GameObject;
            ResourceCache.Add(GameObjectHandle, LoadedObject);
        }
        public bool CacheContains(string Handle)
        {
            return ResourceCache.ContainsKey(Handle);
        }
        public int CacheItemCount
        {
            get
            {
                return ResourceCache.Count;
            }
        }

        private void AddGameEntityCacheEntry(GameObject Entity)
        {
            GameEntityCacheEntry NewCacheEntry = new GameEntityCacheEntry(Entity);
            GameEntityCache.Add(NewCacheEntry);
        }

        private void RemoveGameEntityCacheEntry(GameEntityCacheEntry Entry)
        {
            GameEntityCache.Remove(Entry);
        }
        private void RemoveGameEntityCacheEntry(int EntityHash)
        {
            for(int i = 0; i < GameEntityCache.Count; ++i)
            {
                if(GameEntityCache[i].EntityHash == EntityHash)
                {
                    GameEntityCache.RemoveAt(i);
                    break;
                }
            }
        }
    }
}