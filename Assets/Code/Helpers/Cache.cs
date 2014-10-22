using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class Cache<T>
    {
        private List<T> CacheList;

        public Cache()
        {
            CacheList = new List<T>();
        }
    }
}