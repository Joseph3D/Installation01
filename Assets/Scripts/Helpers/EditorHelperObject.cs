using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
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