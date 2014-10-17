using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using UnityEngine;

namespace Helpers
{
    public sealed class ResourceManager
    {
        private static readonly ResourceManager instance = new ResourceManager();

        private Dictionary<string, object> ResourceCache;

        public object this[string Key]
        {
            get
            {
                return ResourceCache.ContainsKey(Key) ? ResourceCache[Key] : null;
            }
        }

        public static ResourceManager Instance
        {
            get
            {
                return instance;
            }
        }
        static ResourceManager()
        {
            
        }

        private ResourceManager()
        {
            ResourceCache = new Dictionary<string, object>();
        }

        public void LoadGameObject(string File)
        {
            GameObject LoadedObject = Resources.Load(File) as GameObject;
            ResourceCache.Add(File, LoadedObject);
        }

        public void UnloadGameObject(string Key)
        {
            if(ResourceCache.ContainsKey(Key))
            {
                GameObject game_object = ResourceCache[Key] as GameObject;
                Resources.UnloadAsset(game_object);
                ResourceCache.Remove(Key);
            }
        }
    }
}