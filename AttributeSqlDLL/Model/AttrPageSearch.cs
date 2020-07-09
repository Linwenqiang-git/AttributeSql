using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Model
{
    /// <summary>
    /// 页面搜索父类
    /// </summary>
    public class AttrPageSearch
    {
        /// <summary>
        /// 第几页（从1开始）
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 返回的每页行数
        /// </summary>
        public int? Size { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string SortWay { get; set; }
        public virtual int? Offset => (Index - 1) * Size;
    }
}
