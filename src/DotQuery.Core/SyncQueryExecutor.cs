using System;
using DotQuery.Core.Caches;

namespace DotQuery.Core
{
    /// <summary>
    /// Core abstract class of this library that gracefully handles all cache read/hit/write on given query.
    /// All real sync query executor should inherit from this class and implement DoQuerySync.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query object</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    [Obsolete("Please use AsyncQueryExecutor<TQuery, TResult> instead.")]
    public abstract class SyncQueryExecutor<TQuery, TResult>
    {
        private readonly IQueryCache<TQuery, Lazy<TResult>> m_queryTaskCache;

        /// <summary>
        /// Protected constructor of the query executor
        /// </summary>
        /// <param name="queryCache"></param>
        protected SyncQueryExecutor(IQueryCache<TQuery, Lazy<TResult>> queryCache)
        {
            m_queryTaskCache = queryCache;
        }

        /// <summary>
        /// Execute the given query as an synchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>The result of the given query</returns>
        public TResult QuerySync(TQuery query)
        {
            return this.QuerySync(query, EntryOptions.Default);
        }

        /// <summary>
        /// Execute the given query as an synchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="options">Query options to use for this query</param>
        /// <returns>The result of the given query</returns>
        public TResult QuerySync(TQuery query, EntryOptions options)
        {
            if ((options.Behaviors & EntryBehaviors.LookupCache) == EntryBehaviors.LookupCache)
            {
                if ((options.Behaviors & EntryBehaviors.SaveToCache) == EntryBehaviors.SaveToCache)
                {
                    return m_queryTaskCache.GetOrAdd(query, new Lazy<TResult>(() => DoQuerySync(query)), options).Value;
                }
                else
                {
                    Lazy<TResult> lazyResult;
                    if (m_queryTaskCache.TryGet(query, out lazyResult))
                    {
                        return lazyResult.Value;
                    }
                    else
                    {
                        return DoQuerySync(query);
                    }
                }
            }
            else
            {
                if ((options.Behaviors & EntryBehaviors.SaveToCache) == EntryBehaviors.SaveToCache)
                {
                    var newQueryTask = new Lazy<TResult>(() => DoQuerySync(query));
                    m_queryTaskCache.Set(query, newQueryTask, options);
                    return newQueryTask.Value;
                }
                else
                {
                    return DoQuerySync(query);
                }
            }
        }

        /// <summary>
        /// Execute the query in the real world (without cache read/write)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract TResult DoQuerySync(TQuery query);
    }
}
