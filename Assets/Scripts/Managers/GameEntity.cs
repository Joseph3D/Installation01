using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Represents an entity in the game
    /// </summary>
    public class GameEntity : MonoBehaviour
    {
        public string Name { get; protected set; }

        public GameEntity(string EntityName)
        {
            Name = EntityName;
        }

        public void Start()
        {

        }

        public void Update()
        {

        }
    }
}