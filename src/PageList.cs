namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T>
    {
        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevious => PageIndex > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNext => PageIndex < TotalPages;
    }
}
