using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlAttribute.GroupHaving
{
    /// <summary>
    /// 绑定group by 字段，一个字段只允许添加一次
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class GroupByAttribute : Attribute
    {
        private string tableByName;
        private string fieldName;
        /// <summary>
        /// 默认绑定，解析时会按照字段名字解析
        /// </summary>
        public GroupByAttribute()
        {
            tableByName = "";
            fieldName = "";
        }
        public GroupByAttribute(string _fieldName)
        {
            tableByName = "";
            fieldName = _fieldName;
        }
        public GroupByAttribute(string _tableByName, string _fieldName)
        {
            tableByName = _tableByName;
            fieldName = _fieldName;
        }
        public string GetGroupByField()
        {
            if (!string.IsNullOrEmpty(tableByName))
                return $"{tableByName}.{fieldName}";
            else
                return $"{fieldName}";
        }
    }
}
