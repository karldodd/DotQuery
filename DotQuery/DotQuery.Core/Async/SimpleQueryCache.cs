using System.Collections.Generic;

namespace DotQuery.Core.Async
{
    /// <summary>
    /// A simple but working in-memory cache (backed by Dictionary<TKey,V>)
    /// </summary>
    /// <remarks>
    /// This query cache implementation is not thread safe.
    /// </remarks>
    public class SimpleQueryCache<TKey, TValue> : IAsyncQueryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, AsyncLazy<TValue>> m_dictionary;

        public SimpleQueryCache(IEqualityComparer<TKey> keyComparer)
        {
            m_dictionary = new Dictionary<TKey, AsyncLazy<TValue>>(keyComparer);
        }

        public void Set(TKey key, AsyncLazy<TValue> value)
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

        public AsyncLazy<TValue> GetOrAdd(TKey key, AsyncLazy<TValue> lazyTask)
        {
            AsyncLazy<TValue> cached;
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

        public bool TryGet(TKey key, out AsyncLazy<TValue> value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }
    }
}
