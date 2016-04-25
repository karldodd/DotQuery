namespace DotQuery.Core.Caches
{
    public interface IQueryCache<in TKey, TValue>
    {
        TValue GetOrAdd(TKey key, TValue lazyTask);
        bool TryGet(TKey key, out TValue value);
        void Set(TKey key, TValue value);
        void Trim();
        void Clear();
    }
}
