using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using Assets.Scripts.Data;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        #region Members

        #endregion

        void Start()
        {

        }

        void Update()
        {

        }

        private void InitializeInternals()
        {
            GlobalObjectPool.instance.AddObject("GameManager", this);
        }
    }
}