using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Helpers;
using GameLogic;
using Helpers;

using UnityDebug  = UnityEngine.Debug;
using UnityRandom = UnityEngine.Random;

namespace GameLogic
{
    [Serializable]
    public sealed class WeaponTraits
    {
        public int MagazineSize { get; private set; }
        public float Damage { get; private set; }
    }
}