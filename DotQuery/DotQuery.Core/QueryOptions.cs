using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotQuery.Core
{
    /// <summary>
    /// Options to be set about how the underlying cache should do with the given query
    /// </summary>
    [Flags]
    public enum QueryOptions
    {
        None = 0,
        LookupCache = 1,
        CacheResult = 2,
        Default = LookupCache + CacheResult
    }
}
