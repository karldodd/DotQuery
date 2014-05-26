using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Queries;

namespace DotQuery.Core.Test.Stub
{
    public class AddQuery : QueryBase
    {
        public int Left { get; set; }
        public int Right { get; set; }
    }
}
