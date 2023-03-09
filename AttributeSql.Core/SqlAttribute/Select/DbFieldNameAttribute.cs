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
        private string DbFieldName { get; set; }
        private string TimeSuffix { get; set; }
        private bool IsDatetime { get; set; }
        /// <summary>
        /// 字段属性
        /// </summary>
        /// <param name="_DbFieldName">字段名称</param>
        /// <param name="_IsDatetime">是否是时间类型字段</param>
        /// <param name="TimeSuffix">时间类型尾缀</param>
        public DbFieldNameAttribute(string _DbFieldName, bool _IsDatetime = false, string _TimeSuffix = "")
        {
            DbFieldName = _DbFieldName;
            IsDatetime = _IsDatetime;
            TimeSuffix = _TimeSuffix;
        }
        public string GetDbFieldName()
        {
            return DbFieldName;
        }
        public string GetTimeSuffix()
        {
            return TimeSuffix;
        }
        public bool DatetimeField()
        {
            return IsDatetime;
        }
    }
}
