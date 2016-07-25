namespace DotQuery.Core.Caches
{
    public interface IQueryCache<in TKey, TValue>
    {
        TValue GetOrAdd(TKey key, TValue lazyTask);
        TValue GetOrAdd(TKey key, TValue lazyTask, CacheEntryOptions options);
        bool TryGet(TKey key, out TValue value);
        void Set(TKey key, TValue value);
        void Set(TKey key, TValue value, CacheEntryOptions options);
        void Trim();
        void Clear();
    }
}
