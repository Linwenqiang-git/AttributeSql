using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.Select
{
    /// <summary>
    /// 用于设置查询字段在数据库的名字
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbFieldNameAttribute : Attribute
    {
        private string DbFieldName { get; set; }
        public DbFieldNameAttribute(string _DbFieldName)
        {
            DbFieldName = _DbFieldName;
        }
        public string GetDbFieldName()
        {
            return DbFieldName;
        }
    }
}
