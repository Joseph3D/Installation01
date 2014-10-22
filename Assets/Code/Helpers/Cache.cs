using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class Cache<T>
    {
        private List<T> CacheList;

        public int Count
        {
            get
            {
                return CacheList.Count;
            }
        }

        public T this[int Index]
        {
            get
            {
                return CacheList[Index];
            }
        }

        public Cache()
        {
            CacheList = new List<T>();
        }
        public Cache(int MaxCapacity)
        {
            CacheList = new List<T>(MaxCapacity);
        }

        public void Add(T Item)
        {
            CacheList.Add(Item);
        }

        public void Remove(T Item)
        {
            CacheList.Remove(Item);
        }
        public void Remove(int Index)
        {
            CacheList.RemoveAt(Index);
        }

        public T Get(int Index)
        {
            return CacheList[Index];
        }
    }

    public class KeyedCache<K,T>
    {
        private Dictionary<K, T> CacheDictionary;

        public int Count
        {
            get
            {
                return CacheDictionary.Count;
            }
        }

        public T this[K Key]
        {
            get
            {
                return CacheDictionary[Key];
            }
        }

        public KeyedCache()
        {
            CacheDictionary = new Dictionary<K, T>();
        }

        public KeyedCache(int MaxCapacity)
        {
            CacheDictionary = new Dictionary<K, T>(MaxCapacity);
        }

        public bool ContainsKey(K Key)
        {
            return CacheDictionary.ContainsKey(Key);
        }

        public void Add(K Key, T Object)
        {
            CacheDictionary.Add(K, T);
        }
    }
}