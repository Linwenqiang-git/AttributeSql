using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.Model
{
    internal class AttrSqlCacheModel
    {
        public string Select { get; set; }
        public string Join { get; set; }
        public string GroupByHaving { get; set; }
        /// <summary>
        /// 调用次数
        /// </summary>
        public int CallNum { get; set; }
    }
}
