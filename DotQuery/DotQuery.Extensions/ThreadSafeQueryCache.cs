using DotQuery.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;

namespace DotQuery.Extensions
{
    /// <summary>
    /// A simple but working in-memory cache (backed by ConcurrentDictionary<TKey,V>)
    /// </summary>
    public class ThreadSafeQueryCache<TKey, TResult> : IQueryCache<TKey, TResult>
    {
        private readonly ConcurrentDictionary<TKey, TResult> m_dictionary;

        public ThreadSafeQueryCache(IEqualityComparer<TKey> keyComparer)
        {
            m_dictionary = new ConcurrentDictionary<TKey, TResult>(keyComparer);
        }

        public bool TryGet(TKey key, out TResult value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public void Set(TKey key, TResult value)
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

        public TResult GetOrAdd(TKey key, TResult lazyTask)
        {
            return m_dictionary.GetOrAdd(key, lazyTask);
        }
    }
}
