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
        #region Members
        #region Game State Management
        public GameMode InitialMode;

        private GameMode Mode;
        #endregion

        #region Resource Management
        private bool _AssetsLoaded;
        public bool AssetsLoaded
        {
            get { return _AssetsLoaded; }
            set { _AssetsLoaded = value; }
        }

        public string[] StartupAssets;

        private Dictionary<string, object> ResourceCache;
        #endregion
        #endregion

        #region Methods
        void Start()
        {
            InitializeInternals();
        }

        void Update()
        {
            if (!_AssetsLoaded)
                LoadAssets();
        }

        private void UpdateState()
        {

        }

        private void InitializeInternals()
        {
            ResourceCache = new Dictionary<string, object>();
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

        #region Resource Management
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
        #endregion
        #endregion
    }
}