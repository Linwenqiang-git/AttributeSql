using AttributeSql.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Select
{
    /// <summary>
    /// 用于设置查询字段在数据库的名字
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbFieldNameAttribute : Attribute
    {
        private string _dbFieldName;
        private string _timeSuffix;
        private bool _isDatetime;
        /// <summary>
        /// 字段属性
        /// </summary>
        /// <param name="dbFieldName">字段名称</param>
        /// <param name="isDatetime">是否是时间类型字段</param>
        /// <param name="timeSuffix">时间类型尾缀</param>
        public DbFieldNameAttribute(string dbFieldName, bool isDatetime = false, string timeSuffix = "")
        {
            _dbFieldName = dbFieldName;
            _isDatetime = isDatetime;
            _timeSuffix = timeSuffix;
        }
        public string GetDbFieldName()
        {
            return _dbFieldName;
        }
        public string GetTimeSuffix()
        {
            return _timeSuffix;
        }
        public bool DatetimeField()
        {
            return _isDatetime;
        }
    }
}
