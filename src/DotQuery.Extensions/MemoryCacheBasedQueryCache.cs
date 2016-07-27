using System;
using DotQuery.Core;
using DotQuery.Core.Caches;
using Microsoft.Extensions.Caching.Memory;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private readonly IKeySerializer<TKey> _keySerializer;
        private readonly TimeSpan _slidingExpiration;
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public MemoryCacheBasedQueryCache(IKeySerializer<TKey> keySerializer, TimeSpan slidingExpiration)
        {
            _keySerializer = keySerializer;
            _slidingExpiration = slidingExpiration;
        }

        public void Trim()
        {
            _memoryCache.Compact(75);
        }

        public void Clear()
        {
            _memoryCache.Compact(100);
        }

        public TValue GetOrAdd(TKey key, TValue lazyTask, EntryOptions options)
        {
            string serializeKey = GetSerializeKey(key);
            return _memoryCache.GetOrCreate(
                serializeKey,
                e =>
                {
                    e.AbsoluteExpiration = options.AbsoluteExpiration;
                    e.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                    e.SlidingExpiration = CalculateSlidingExpiration(options);
                    return lazyTask;
                });
        }

        public bool TryGet(TKey key, out TValue value)
        {
            string serializeKey = GetSerializeKey(key);
            object cached = _memoryCache.Get(serializeKey);

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

        public void Set(TKey key, TValue value, EntryOptions options)
        {
            string serializeKey = GetSerializeKey(key);
            _memoryCache.Set(serializeKey, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = CalculateSlidingExpiration(options)
            });
        }

        private string GetSerializeKey(TKey key)
        {
            return _keySerializer.SerializeToString(key);
        }

        private TimeSpan? CalculateSlidingExpiration(EntryOptions options)
        {
            return options == EntryOptions.Default ? _slidingExpiration : options.SlidingExpiration;
        }
    }
}
