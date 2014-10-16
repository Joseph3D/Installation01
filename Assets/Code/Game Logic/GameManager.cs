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
    }
}