using System;
using DotQuery.Core;
using DotQuery.Core.Caches;
using Microsoft.Extensions.Caching.Memory;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private readonly IKeySerializer<TKey> _keySerializer;
        private readonly CacheEntryOptions _slidingExpirationOptions;
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public MemoryCacheBasedQueryCache(IKeySerializer<TKey> keySerializer, TimeSpan slidingExpiration)
        {
            _keySerializer = keySerializer;
            _slidingExpirationOptions = new CacheEntryOptions { SlidingExpiration = slidingExpiration };
        }

        public void Trim()
        {
            _memoryCache.Compact(75);
        }

        public void Clear()
        {
            _memoryCache.Compact(100);
        }

        public TValue GetOrAdd(TKey key, TValue lazyTask) => GetOrAdd(key, lazyTask, _slidingExpirationOptions);

        public TValue GetOrAdd(TKey key, TValue lazyTask, CacheEntryOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var entry = _memoryCache.CreateEntry(key);
            return _memoryCache.GetOrCreate(
                _keySerializer.SerializeToString(key),
                e =>
                {
                    e.AbsoluteExpiration = options.AbsoluteExpiration;
                    e.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                    e.SlidingExpiration = options.SlidingExpiration;
                    e.Value = lazyTask;
                    return lazyTask;
                });
        }

        public bool TryGet(TKey key, out TValue value)
        {
            object cached = _memoryCache.Get(_keySerializer.SerializeToString(key));

            if (cached == null)
            {
                value = default(TValue);
                return false;
            }
            else
            {
                value = (TValue)cached;
                return true;
            }
        }

        public void Set(TKey key, TValue value) => Set(key, value, _slidingExpirationOptions);

        public void Set(TKey key, TValue value, CacheEntryOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            string keyAsString = _keySerializer.SerializeToString(key);
            _memoryCache.Set(keyAsString, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            });
        }
    }
}
