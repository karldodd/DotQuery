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
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IAsyncQueryCache<TKey, TValue>
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

        public AsyncLazy<TValue> GetOrAdd(TKey key, AsyncLazy<TValue> lazyTask)
        {
            var existing = m_objectCache.AddOrGetExisting(new CacheItem(key.ToString(), lazyTask), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            return existing == null ? lazyTask : (AsyncLazy<TValue>)existing.Value;
        }

        public bool TryGet(TKey key, out AsyncLazy<TValue> value)
        {
            object cached = m_objectCache.Get(key.ToString());

            if (cached == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = (AsyncLazy<TValue>)cached;
                return true;
            }
        }

        public void Set(TKey key, AsyncLazy<TValue> value)
        {
            m_objectCache.Set(new CacheItem(key.ToString(), value), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            m_objectCache[key.ToString()] = value;
        }

    }
}
