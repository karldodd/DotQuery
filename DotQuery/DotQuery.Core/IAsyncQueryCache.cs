using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public interface IAsyncQueryCache<in TKey, TValue>
    {
        AsyncLazy<TValue> GetOrAdd(TKey key, AsyncLazy<TValue> lazyTask);
        bool TryGet(TKey key, out AsyncLazy<TValue> value);
        void Set(TKey key, AsyncLazy<TValue> value);
        void Trim();
        void Clear();
    }
}
