using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public interface IQueryProducer<out TQuery> where TQuery : QueryBase
    {
        TQuery ProduceQuery();
        bool SupportsExport { get; }
        TQuery ProduceExportQuery();
    }
}
