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
    public static class GameManagerLocator
    {
        private static GameManager _Manager;
        private static bool ManagerFound = false;

        public static GameManager Manager
        {
            get
            {
                return _Manager;
            }
        }

        static GameManagerLocator()
        {
            if(!ManagerFound)
            {
                _Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                ManagerFound = true;
            }
        }
    }
}