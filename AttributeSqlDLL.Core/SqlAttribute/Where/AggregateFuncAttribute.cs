using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.Where
{
    /// <summary>
    /// 聚合函数
    /// </summary>
    public class AggregateFuncAttribute : Attribute
    {
        private string funcoperate;
        public AggregateFuncAttribute(string FuncName)
        {
            funcoperate = FuncName;
        }
        public string GetFuncoperate()
        {
            return funcoperate;
        }
    }
}
