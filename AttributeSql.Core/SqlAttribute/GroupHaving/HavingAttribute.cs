using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.GroupHaving
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class HavingAttribute : Attribute
    {
        private string _havingCondition;
        public HavingAttribute(string havingCondition)
        {
            _havingCondition = havingCondition;
        }
        public string GetHavingCondition()
        {
            return _havingCondition;
        }
    }
}
