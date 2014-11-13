using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(EntityTag))]
    public sealed class Grenade : MonoBehaviour
    {
        private GameManager Manager;

        private EntityTag Tag;
        private Rigidbody Body;

        public void Awake()
        {

        }

        public void Start()
        {

        }

        public void Update()
        {

        }

        public void FixedUpdate()
        {

        }
    }
}