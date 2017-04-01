using System;
using DotQuery.Core;
using DotQuery.Core.Caches;
using Microsoft.Extensions.Caching.Memory;

namespace DotQuery.Extensions
{
    public class MemoryCacheBasedQueryCache<TKey, TValue> : IQueryCache<TKey, TValue>
    {
        private readonly IKeySerializer<TKey> _keySerializer;
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public MemoryCacheBasedQueryCache(IKeySerializer<TKey> keySerializer)
        {
            _keySerializer = keySerializer;
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
            TValue result;
            if (!_memoryCache.TryGetValue(serializeKey, out result))
            {
                result = lazyTask;
                var memoryEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = options.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = options.SlidingExpiration,
                };
                _memoryCache.Set(serializeKey, result, memoryEntryOptions);
            }
            return result;
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
                SlidingExpiration = options.SlidingExpiration
            });
        }

        public void Remove(TKey key)
        {
            string serializeKey = GetSerializeKey(key);
            _memoryCache.Remove(key);
        }

        private string GetSerializeKey(TKey key)
        {
            return _keySerializer.SerializeToString(key);
        }
    }
}
