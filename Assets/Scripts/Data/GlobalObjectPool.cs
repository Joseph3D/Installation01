using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Represents a globally accesable object
    /// </summary>
    public class GlobalObjectPool
    {
        private static Dictionary<string, object> Pool;

        public static GlobalObjectPool instance;


        public object this[string ObjectName]
        {
            get
            {
                return Pool[ObjectName];
            }
        }

        /// <summary>
        /// Gets the number of objects in the pool
        /// </summary>
        public int PoolSize
        {
            get
            {
                return Pool.Count;
            }
        }

        /// <summary>
        /// GlobalObjectPool constructor
        /// </summary>
        public GlobalObjectPool()
        {
            instance = this;

            Pool = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds an object to the pool
        /// </summary>
        /// <param name="Name">name of the object</param>
        /// <param name="Handle">The System.Object to be added</param>
        public void AddObject(string Name, object Handle)
        {
            Pool.Add(Name, Handle);
        }

        /// <summary>
        /// Removes an object from the pool
        /// </summary>
        /// <param name="Name">Name of the object to remove</param>
        public void RemoveObject(string Name)
        {
            Pool.Remove(Name);
        }

        /// <summary>
        /// Returns true if there is an object in the pool with the specified name
        /// </summary>
        /// <param name="Name">Name of the object</param>
        /// <returns></returns>
        public bool ObjectInPool(string Name)
        {
            return Pool.ContainsKey(Name);
        }
    }
}