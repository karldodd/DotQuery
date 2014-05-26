using DotQuery.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private MemoryCache m_objectCache = MemoryCache.Default;

        public MemoryCacheBasedQueryCache()
        {
        }
        
        public void Trim()
        {
            m_objectCache.Trim(75);
        }

        public void Clear()
        {
            m_objectCache.Trim(100);
        }

        public TValue GetOrAdd(TKey key, TValue lazyTask)
        {
            var existing = m_objectCache.AddOrGetExisting(new CacheItem(key.ToString(), lazyTask), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            return existing == null ? lazyTask : (TValue)existing.Value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            object cached = m_objectCache.Get(key.ToString());

            if (cached == null)
            {
                value = default(TValue);
                return false;
            }
            else
            {
                value = (TValue)cached;
                return true;
            }
        }

        public void Set(TKey key, TValue value)
        {
            m_objectCache.Set(new CacheItem(key.ToString(), value), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            m_objectCache[key.ToString()] = value;
        }

    }
}
