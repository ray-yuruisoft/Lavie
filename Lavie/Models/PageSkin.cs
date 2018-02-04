namespace Lavie.Models
{
    public class PageSkin : IPageSkin
    {
        public string LinkFormat { get; private set; }
        public string InfoFormat { get; private set; }
        public string CurrentLinkFormat { get; private set; }

        public string FirstLinkText { get; private set; }
        public string FirstLinkDisabledText { get; private set; }
        public string PreviousLinkText { get; private set; }
        public string PreviousLinkDisabledText { get; private set; }
        public string NextLinkText { get; private set; }
        public string NextLinkDisabledText { get; private set; }
        public string LastLinkText { get; private set; }
        public string LastLinkDisabledText { get; private set; }

        public string LinkSeparator { get; private set; }

        public PageSkin(){
            LinkFormat = "<a href=\"{0}\">{1}</a>";
            InfoFormat = "第 {0}/{1} 页&nbsp;共 {2} 条&nbsp;每页显示 {3} 条&nbsp;&nbsp;";
            CurrentLinkFormat = "<span class=\"current\">{0}</span>";

            FirstLinkText = "<span>|&lt;</span>";
            FirstLinkDisabledText = "<span class=\"disabled\">|&lt;</span>";
            PreviousLinkText = "<span>&lt;</span>";
            PreviousLinkDisabledText = "<span class=\"disabled\">&lt;</span>";
            NextLinkText = "<span>&gt;</span>";
            NextLinkDisabledText = "<span class=\"disabled\">&gt;</span>";
            LastLinkText = "<span>&gt;|</span>";
            LastLinkDisabledText = "<span class=\"disabled\">&gt;|</span>";

            LinkSeparator = "…";
        }
    }
    public interface IPageSkin
    {
        /// <summary>
        /// 分页链接格式
        /// </summary>
        string LinkFormat { get; }
        /// <summary>
        /// 分页信息格式（总条数、总页数、当前页、每页显示数）
        /// </summary>
        string InfoFormat { get; }
        /// <summary>
        /// 当前页格式
        /// </summary>
        string CurrentLinkFormat { get; }

        /// <summary>
        /// 第一页链接文本
        /// </summary>
        string FirstLinkText { get; }
        /// <summary>
        /// 第一页链接不可用文本（当前页为第一页时，不可用）
        /// </summary>
        string FirstLinkDisabledText { get; }
        /// <summary>
        /// 上一页链接文本
        /// </summary>
        string PreviousLinkText { get; }
        /// <summary>
        /// 上一页链接不可用文本（当前页为最末页时，不可用）
        /// </summary>
        string PreviousLinkDisabledText { get; }
        /// <summary>
        /// 下一页链接文本
        /// </summary>
        string NextLinkText { get; }
        /// <summary>
        /// 下一页链接不可用文本（当前页为最末页时，不可用）
        /// </summary>
        string NextLinkDisabledText { get; }
        /// <summary>
        /// 最末页链接文本
        /// </summary>
        string LastLinkText { get; }
        /// <summary>
        /// 最末页链接不可用文本（当前页为最末页时，不可用）
        /// </summary>
        string LastLinkDisabledText { get; }

        /// <summary>
        /// 分页分隔符
        /// </summary>
        string LinkSeparator { get; }
    }
}
