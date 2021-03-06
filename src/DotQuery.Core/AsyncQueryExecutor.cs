﻿using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;

namespace DotQuery.Core
{
    /// <summary>
    /// Core abstract class of this library that gracefully handles all cache read/hit/write on given query.
    /// All real query executor should inherit from this class and implement DoQueryAsync.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query object</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public abstract class AsyncQueryExecutor<TQuery, TResult> : IAsyncQueryExecutor<TQuery, TResult>
    {
        private readonly IQueryCache<TQuery, AsyncLazy<TResult>> m_queryTaskCache;

        /// <summary>
        /// Protected constructor of the query executor
        /// </summary>
        /// <param name="queryCache"></param>
        protected AsyncQueryExecutor(IQueryCache<TQuery, AsyncLazy<TResult>> queryCache)
        {
            m_queryTaskCache = queryCache;
        }

        /// <summary>
        /// Execute the given query as an asynchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task<TResult> QueryAsync(TQuery query)
        {
            return this.QueryAsync(query, EntryOptions.Default);
        }

        /// <summary>
        /// Execute the given query as an asynchronous operation with a specified cache policy.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="options">The cache policy options.</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task<TResult> QueryAsync(TQuery query, EntryOptions options)
        {
            Task<TResult> queryTask = null;
            if ((options.Behaviors & EntryBehaviors.LookupCache) == EntryBehaviors.LookupCache)
            {
                if ((options.Behaviors & EntryBehaviors.SaveToCache) == EntryBehaviors.SaveToCache)
                {
                    queryTask = m_queryTaskCache.GetOrAdd(query, new AsyncLazy<TResult>(() => DoQueryAsync(query)), options).Value;
                }
                else
                {
                    AsyncLazy<TResult> asyncLazyResult;
                    if (m_queryTaskCache.TryGet(query, out asyncLazyResult))
                    {
                        queryTask = asyncLazyResult.Value;
                    }
                    else
                    {
                        //no task cached, do it directly
                        return DoQueryAsync(query);
                    }
                }

                //re-query if the task is canceld or failed.
                if ((queryTask.IsFaulted || queryTask.IsCanceled) && (options.Behaviors & EntryBehaviors.ReQueryWhenErrorCached) == EntryBehaviors.ReQueryWhenErrorCached)
                {
                    options.Behaviors ^= EntryBehaviors.LookupCache;
                    this.QueryAsync(query, options);
                }

                //query is cached, just await the result
                return queryTask;
            }
            else
            {
                if ((options.Behaviors & EntryBehaviors.SaveToCache) == EntryBehaviors.SaveToCache)
                {
                    var newQueryTask = new AsyncLazy<TResult>(() => DoQueryAsync(query));
                    m_queryTaskCache.Set(query, newQueryTask, options);
                    return newQueryTask.Value;
                }
                else
                {
                    return DoQueryAsync(query);
                }
            }
        }

        /// <summary>
        /// Execute the query in the real world (without cache read/write)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract Task<TResult> DoQueryAsync(TQuery query);
    }
}
