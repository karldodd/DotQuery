using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    /// <summary>
    /// A simple but working in-memory cache (backed by Dictionary<TKey,V>)
    /// </summary>
    /// <remarks>
    /// This query cache implementation is not thread safe.
    /// </remarks>
    public class SimpleQueryCache<TKey> : IQueryCache<TKey>
    {
        private readonly Dictionary<TKey, object> m_dictionary;

        public SimpleQueryCache(IEqualityComparer<TKey> keyComparer)
        {
            m_dictionary = new Dictionary<TKey, object>(keyComparer);
        }

        public bool TryGetFromCache(TKey key, out object value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public void CacheValue(TKey key, object value)
        {
            m_dictionary[key] = value;
        }

        public void Trim()
        {
            m_dictionary.Clear();
        }

        public void Clear()
        {
            m_dictionary.Clear();
        }
    }
}
