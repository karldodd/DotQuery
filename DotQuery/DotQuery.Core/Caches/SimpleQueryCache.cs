using System.Collections.Generic;

namespace DotQuery.Core.Caches
{
    /// <summary>
    /// A simple but working in-memory cache (backed by Dictionary<TKey,TValue>)
    /// </summary>
    /// <remarks>
    /// This query cache implementation is not thread safe.
    /// </remarks>
    public class SimpleQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_dictionary;

        public SimpleQueryCache(IEqualityComparer<TKey> keyComparer)
        {
            m_dictionary = new Dictionary<TKey, TValue>(keyComparer);
        }

        public void Set(TKey key, TValue value)
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

        public TValue GetOrAdd(TKey key, TValue lazyTask)
        {
            TValue cached;
            if (m_dictionary.TryGetValue(key, out cached))
            {
                return cached;
            }
            else
            {
                m_dictionary[key] = lazyTask;
                return lazyTask;
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }
    }
}
