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
        private bool _CriticalAssetsLoaded;
        public bool CriticalAssetsLoaded
        {
            get { return _CriticalAssetsLoaded; }
            set { _CriticalAssetsLoaded = value; }
        }

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
            
        }

        private void UpdateState()
        {

        }

        private void InitializeInternals()
        {
            ResourceCache = new Dictionary<string, object>();
        }

        private void LoadCriticalAssets()
        {

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
        #endregion
        #endregion
    }
}