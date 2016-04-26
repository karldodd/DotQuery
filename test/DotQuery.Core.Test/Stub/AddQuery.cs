using DotQuery.Core.Queries;

namespace DotQuery.Core.Test.Stub
{
    public class AddQuery : QueryBase
    {
        public int Left { get; set; }
        public int Right { get; set; }

        public override string ToString()
        {
            return string.Format("{0}+{1}", Left, Right);
        }
    }
}
