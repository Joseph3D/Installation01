using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using GameLogic;
using Helpers;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        #region Members
        public GameMode InitialMode;

        private GameMode Mode;
        #endregion

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
        }

        private void LoadCriticalAssets()
        {

        }

        /// <summary>
        /// Loads an asset via the resource manager and returns
        /// </summary>
        /// <param name="AssetFile"></param>
        /// <returns></returns>
        public object LoadAsset(string AssetFile)
        {
            ResourceManager.Instance.LoadGameObject(AssetFile);
            return ResourceManager.Instance[AssetFile];
        }

        public object LoadAsset(string AssetFile, string AssetName)
        {

        }
    }
}