using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    /// <summary>
    /// Query executor that goes through a cache (also uses async await)
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"> </typeparam>
    public abstract class QueryExecutor<TQuery, TResult> where TQuery : QueryBase
    {
        private readonly IQueryCache m_queryCache;

        protected QueryExecutor(IQueryCache queryCache)
        {
            m_queryCache = queryCache;
        }

        public class QueryCacheException : Exception
        {
            public QueryCacheException(string msg)
                : base(msg)
            {
            }
        }

        public async Task<TResult> QueryAsync(TQuery query)
        {
            object cachedValue;
            if (query.QueryOptions.HasFlag(QueryOptions.LookupCache) && m_queryCache.TryGetFromCache(query, out cachedValue))
            {
                var queryTask = cachedValue as Task<TResult>;
                if (queryTask != null)
                {
                    //re-query if the task is canceld or failed.
                    //todo: make this configurable
                    if (queryTask.IsFaulted || queryTask.IsCanceled)
                    {
                        return await QueryAsyncWithoutCacheLookup(query).ConfigureAwait(false);
                    }

                    //query is cached, just await the result
                    return await queryTask.ConfigureAwait(false);
                }

                if (cachedValue is TResult)
                {
                    var cachedStream = cachedValue as Stream;

                    if (cachedStream != null)
                    {
                        //special treatment if the cached object is a stream
                        if (cachedStream.CanSeek) //reusable, probably memory stream
                        {
                            cachedStream.Seek(0, SeekOrigin.Begin);
                            return (TResult)cachedValue;
                        }
                        else
                        {
                            //The stream is probably used... Dispose it and query again...
                            //todo: make cache stream configurable
                            cachedStream.Dispose();
                            return await QueryAsyncWithoutCacheLookup(query).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        //result is cached
                        return (TResult)cachedValue;
                    }
                }

                throw new QueryCacheException("An unintended result was cached in query cache: " + cachedValue);
            }
            else
            {
                return await QueryAsyncWithoutCacheLookup(query).ConfigureAwait(false);
            }
        }

        protected async Task<TResult> QueryAsyncWithoutCacheLookup(TQuery query)
        {
            Task<TResult> task = DoQueryAsync(query);

            bool cacheResult = query.QueryOptions.HasFlag(QueryOptions.CacheResult);
            if (cacheResult) m_queryCache.CacheValue(query, task); //cache the task
            TResult result = await task.ConfigureAwait(false);
            if (cacheResult) m_queryCache.CacheValue(query, result); //cache the result
            return result;
        }

        protected abstract Task<TResult> DoQueryAsync(TQuery query);
    }
}
