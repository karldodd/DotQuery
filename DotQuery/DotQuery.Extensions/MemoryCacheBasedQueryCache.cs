using DotQuery.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TValue> : IAsyncQueryCache<CacheKey, TValue>
    {
        private MemoryCache m_objectCache = MemoryCache.Default;

        public MemoryCacheBasedQueryCache()
        {
        }

        public bool TryGet(CacheKey key, out object value)
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

        public void Set(CacheKey key, object value)
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

        public AsyncLazy<TValue> GetOrAdd(CacheKey key, AsyncLazy<TValue> lazyTask)
        {
            var existing = m_objectCache.AddOrGetExisting(new CacheItem(key.StringKey, lazyTask), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            return existing == null ? lazyTask : (AsyncLazy<TValue>)existing.Value;
        }

        public bool TryGet(CacheKey key, out AsyncLazy<TValue> value)
        {
            object cached = m_objectCache.Get(key.StringKey);

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

        public void Set(CacheKey key, AsyncLazy<TValue> value)
        {
            m_objectCache.Set(new CacheItem(key.StringKey, value), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            m_objectCache[key.StringKey] = value;
        }
    }
}
