using DotQuery.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;

namespace DotQuery.Extensions
{
    /// <summary>
    /// A simple but working in-memory cache (backed by ConcurrentDictionary<TKey,V>)
    /// </summary>
    public class ThreadSafeQueryCache<TKey, TResult> : IAsyncQueryCache<TKey, TResult>
    {
        private readonly ConcurrentDictionary<TKey, AsyncLazy<TResult>> m_dictionary;

        public ThreadSafeQueryCache(IEqualityComparer<TKey> keyComparer)
        {
            m_dictionary = new ConcurrentDictionary<TKey, AsyncLazy<TResult>>(keyComparer);
        }

        public bool TryGet(TKey key, out AsyncLazy<TResult> value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public void Set(TKey key, AsyncLazy<TResult> value)
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

        public AsyncLazy<TResult> GetOrAdd(TKey key, AsyncLazy<TResult> lazyTask)
        {
            return m_dictionary.GetOrAdd(key, lazyTask);
        }
    }
}
