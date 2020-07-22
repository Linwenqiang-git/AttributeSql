using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.JoinTable
{
    /// <summary>
    /// 设置属性的表别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableByNameAttribute : Attribute
    {
        private string ByName;
        public TableByNameAttribute(string ByName)
        {
            this.ByName = ByName;
        }
        public string GetName()
        {
            return ByName;
        }
    }
}
