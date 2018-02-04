namespace Lavie.Models
{
    public class CachePartition
    {
        public CachePartition(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public override string ToString()
        {
            return string.Format("|PageIndex:{0},PageSize:{1}", PageIndex, PageSize);
        }
    }
}
