using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.GroupHaving
{
    /// <summary>
    /// 绑定group by 字段，一个字段只允许添加一次
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class GroupByAttribute : Attribute
    {
        private string _tableByName;
        private string _fieldName;
        /// <summary>
        /// 默认绑定，解析时会按照字段名字解析
        /// </summary>
        public GroupByAttribute()
        {
            _tableByName = string.Empty;
            _fieldName = string.Empty;
        }
        public GroupByAttribute(string fieldName):this()
        {            
            _fieldName = fieldName;
        }
        public GroupByAttribute(string tableByName, string fieldName)
        {
            _tableByName = tableByName;
            _fieldName = fieldName;
        }
        public string GetGroupByField()
        {
            if (!string.IsNullOrEmpty(_tableByName))
                return $"{_tableByName}.{_fieldName}";
            else
                return $"{_fieldName}";
        }
    }
}
