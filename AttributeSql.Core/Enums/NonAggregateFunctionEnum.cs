using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Enums
{
    /// <summary>
    /// 非聚合函数枚举
    /// </summary>
    public enum NonAggregateFunctionEnum
    {
        /// <summary>
        /// 字符串连接函数
        /// </summary>
        [Description("CONCAT")]
        Concat = 0,
        [Description("GROUP_CONCAT")]
        Group_Concat,
        [Description("DATE_FORMAT")]
        Date_Foramt
    }
}
