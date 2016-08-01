namespace DotQuery.Core.Caches
{
    public interface IQueryCache<in TKey, TValue>
    {
        bool TryGet(TKey key, out TValue value);

        TValue GetOrAdd(TKey key, TValue lazyTask, EntryOptions options);

        void Set(TKey key, TValue value, EntryOptions options);

        void Trim();

        void Clear();
    }
}
