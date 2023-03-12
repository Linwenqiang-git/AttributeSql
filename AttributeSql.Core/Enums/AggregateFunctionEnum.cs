using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Enums
{
    /// <summary>
    /// 聚合函数枚举
    /// </summary>
    public enum AggregateFunctionEnum
    {
        [Description("Max")]
        Max = 0,
        [Description("Min")]
        Min,
        [Description("Sum")]
        Sum,
        [Description("Count")]
        Count,
    }
}
