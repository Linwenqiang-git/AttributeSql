using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Model
{
    /// <summary>
    /// 通用返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttrPageResult<T>
       where T : class
    {
        public AttrPageResult()
        {

        }

        public AttrPageResult(int? index, int? size)
        {
            Index = index;
            Size = size;
        }
        public int? Index { get; set; }
        public int? Size { get; set; }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public IEnumerable<T> Rows { get; set; }
    }
}
