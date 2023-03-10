using AttributeSql.Core.Enums;
using AttributeSql.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Where
{
    /// <summary>
    /// 聚合函数
    /// </summary>
    public class AggregateFuncAttribute : Attribute
    {
        private AggregateFunctionEnum _aggregateFunc;
        public AggregateFuncAttribute(AggregateFunctionEnum aggregate)
        {
            _aggregateFunc = aggregate;
        }
        /// <summary>
        /// 获取聚合方法
        /// </summary>
        /// <returns></returns>
        public string GetAggregateFunc()
        {
            return _aggregateFunc.GetDescription();
        }
    }
}
