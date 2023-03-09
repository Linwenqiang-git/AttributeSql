using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.CudAttr
{
    /// <summary>
    /// 数据库字段映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbFiledMappingAttribute : Attribute
    {
        /// <summary>
        /// 数据库字段名
        /// </summary>
        private string DbFieldName;
        /// <summary>
        /// 是否为更新条件
        /// </summary>
        private bool IsCondition;
        /// <summary>
        /// 是否允许更新成空值
        /// </summary>
        private bool IsAllowEmpty;
        public DbFiledMappingAttribute(string _DbFieldName)
        {
            DbFieldName = _DbFieldName;
            IsCondition = false;
            IsAllowEmpty = false;
        }
        public DbFiledMappingAttribute(string _DbFieldName, bool _IsCondition)
        {
            DbFieldName = _DbFieldName;
            IsCondition = _IsCondition;
            IsAllowEmpty = false;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_DbFieldName">数据库字段名</param>
        /// <param name="_IsCondition">是否是删除条件(默认不是)</param>
        /// <param name="_IsAllowEmpty">是否允许更新成null(默认不允许)</param>
        public DbFiledMappingAttribute(string _DbFieldName, bool _IsCondition, bool _IsAllowEmpty)
        {
            DbFieldName = _DbFieldName;
            IsCondition = _IsCondition;
            IsAllowEmpty = _IsAllowEmpty;
        }
        public string GetDbFieldName()
        {
            return DbFieldName;
        }
        public bool GetIsCondition()
        {
            return IsCondition;
        }
        public bool GetIsAllowEmpty()
        {
            return IsAllowEmpty;
        }
    }
}
