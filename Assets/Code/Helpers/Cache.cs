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

        public void Add(T Item)
        {
            CacheList.Add(Item);
        }

        public void Remove(T Item)
        {
            CacheList.Remove(Item);
        }
    }
}