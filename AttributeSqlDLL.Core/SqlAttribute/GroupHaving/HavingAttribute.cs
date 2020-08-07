using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.GroupHaving
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class HavingAttribute : Attribute
    {
        private string havingCondition;
        public HavingAttribute(string _havingCondition)
        {
            havingCondition = _havingCondition;
        }
        public string GetHavingCondition()
        {
            return havingCondition;
        }
    }
}
