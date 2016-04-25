using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core.Caches
{
    /// <summary>
    /// A query cache that never caches anything
    /// </summary>
    /// <remarks>
    /// This query cache implementation is thread safe.
    /// </remarks>
    public class EmptyQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        internal EmptyQueryCache()
        {           
        }

        public static EmptyQueryCache<TKey, TValue> Instance = new EmptyQueryCache<TKey, TValue>();

        public void Set(TKey key, TValue value)
        {            
        }

        public void Trim()
        {         
        }

        public void Clear()
        {         
        }

        public TValue GetOrAdd(TKey key, TValue lazyTask)
        {
            return lazyTask;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }
    }
}
