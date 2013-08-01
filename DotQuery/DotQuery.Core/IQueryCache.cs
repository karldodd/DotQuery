﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public interface IQueryCache
    {
        bool TryGetFromCache(CacheKey key, out object value);
        void CacheValue(CacheKey key, object value);
        void Trim();
        void Clear();
    }
}
