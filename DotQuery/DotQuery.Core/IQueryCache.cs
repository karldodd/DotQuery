using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public interface IQueryCache<in TKey>
    {
        bool TryGetFromCache(TKey key, out object value);
        void CacheValue(TKey key, object value);
        void Trim();
        void Clear();
    }
}
