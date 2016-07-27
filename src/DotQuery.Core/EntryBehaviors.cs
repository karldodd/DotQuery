using System;

namespace DotQuery.Core
{
    /// <summary>
    /// Options to be set about how the underlying cache should do with the given query
    /// </summary>
    [Flags]
    public enum EntryBehaviors
    {
        /// <summary>
        /// The query does nothing related to the cache (raw execution every time)
        /// </summary>
        None = 0,

        /// <summary>
        /// Look up the cache to see if the query's result/task is already there
        /// </summary>
        LookupCache = 1,

        /// <summary>
        /// Save result/task to the cache when the query has been executed or is being executed
        /// </summary>
        SaveToCache = 2,

        /// <summary>
        /// The query should be executed again if the cache contains a failed task
        /// </summary>
        ReQueryWhenErrorCached = 4,

        /// <summary>
        /// The default smart behavior: the query will lookup and save to cache, and re-execute the query if error is cached
        /// </summary>
        Default = LookupCache + SaveToCache + ReQueryWhenErrorCached
    }
}
