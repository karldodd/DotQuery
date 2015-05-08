using DotQuery.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private readonly IKeySerializer<TKey> m_keySerializer;
        private MemoryCache m_objectCache = MemoryCache.Default;

        public MemoryCacheBasedQueryCache(IKeySerializer<TKey> keySerializer)
        {
            this.m_keySerializer = keySerializer;
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
            var existing = m_objectCache.AddOrGetExisting(new CacheItem(m_keySerializer.SerializeToString(key), lazyTask), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            return existing == null ? lazyTask : (TValue)existing.Value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            object cached = m_objectCache.Get(m_keySerializer.SerializeToString(key));

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
            string keyAsString = m_keySerializer.SerializeToString(key);
            m_objectCache.Set(new CacheItem(keyAsString, value), new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2) });
            m_objectCache[keyAsString] = value;
        }

    }
}
