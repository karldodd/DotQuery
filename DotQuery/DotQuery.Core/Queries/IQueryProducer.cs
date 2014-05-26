namespace DotQuery.Core.Queries
{
    public interface IQueryProducer<out TQuery> where TQuery : QueryBase
    {
        TQuery ProduceQuery();
        bool SupportsExport { get; }
        TQuery ProduceExportQuery();
    }
}
