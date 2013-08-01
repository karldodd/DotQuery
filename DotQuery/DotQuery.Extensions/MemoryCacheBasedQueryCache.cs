using DotQuery.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache : IQueryCache
    {
        private MemoryCache m_objectCache = MemoryCache.Default;

        public MemoryCacheBasedQueryCache()
        {
        }

        public bool TryGetFromCache(CacheKey key, out object value)
        {
            object cached = m_objectCache.Get(key.StringKey);

            if (cached == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = cached;
                return true;
            }
        }

        public void CacheValue(CacheKey key, object value)
        {
            m_objectCache.Set(new CacheItem(key.StringKey, value), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            m_objectCache[key.StringKey] = value;
        }

        public void Trim()
        {
            m_objectCache.Trim(75);
        }

        public void Clear()
        {
            m_objectCache.Trim(100);
        }
    }
}
