using DotQuery.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Extensions
{
    /// <summary>
    /// A simple but working in-memory cache (backed by Dictionary<T,V>)
    /// </summary>
    /// <remarks>
    /// This query cache implementation is not thread safe!
    /// </remarks>
    public class ThreadSafeQueryCache : IQueryCache
    {
        private readonly ConcurrentDictionary<CacheKey, object> m_dictionary;

        public ThreadSafeQueryCache(IEqualityComparer<CacheKey> keyComparer)
        {
            m_dictionary = new ConcurrentDictionary<CacheKey, object>(keyComparer);
        }

        public bool TryGetFromCache(CacheKey key, out object value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public void CacheValue(CacheKey key, object value)
        {
            m_dictionary[key] = value;
        }

        public void Trim()
        {
            m_dictionary.Clear();
        }
    }
}
