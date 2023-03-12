using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    /// <summary>
    /// 设置属性的表别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableByNameAttribute : Attribute
    {
        private string _byName;
        public TableByNameAttribute(string byName)
        {
            _byName = byName;
        }
        public string GetName()
        {
            return _byName;
        }
    }
}
