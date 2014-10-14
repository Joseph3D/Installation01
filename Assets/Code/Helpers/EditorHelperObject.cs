using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Represents an object that is visible in the editor
    /// But is rendered invisible when the game starts, WITHOUT removing the underlying GameObject
    /// Intented use is for things like spawn points, AI unit patrol points, etc.
    /// </summary>
    public class EditorHelperObject : MonoBehaviour
    {
        private MeshRenderer Renderer;
        private bool UpdateStarted;

        public void Start()
        {
            UpdateStarted = false;

            Renderer = GetComponent<MeshRenderer>() as MeshRenderer;
        }

        public void Update()
        {
            if (!UpdateStarted)
            {
                Renderer.enabled = false;
                UpdateStarted = true;
            }
        }
    }
}