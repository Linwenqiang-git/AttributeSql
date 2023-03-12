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
        private string _dbFieldName;
        /// <summary>
        /// 是否为更新条件
        /// </summary>
        private bool _isCondition;
        /// <summary>
        /// 是否允许更新成空值
        /// </summary>
        private bool _isAllowEmpty;
        private DbFiledMappingAttribute()
        {
            _isCondition = false;
            _isAllowEmpty = false;
        }
        public DbFiledMappingAttribute(string dbFieldName):this()
        {
            _dbFieldName = dbFieldName;            
        }
        public DbFiledMappingAttribute(string dbFieldName, bool isCondition) : this()
        {
            _dbFieldName = dbFieldName;
            _isCondition = isCondition;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_DbFieldName">数据库字段名</param>
        /// <param name="_IsCondition">是否是删除条件(默认不是)</param>
        /// <param name="_IsAllowEmpty">是否允许更新成null(默认不允许)</param>
        public DbFiledMappingAttribute(string dbFieldName, bool isCondition, bool isAllowEmpty)
        {
            _dbFieldName = dbFieldName;
            _isCondition = isCondition;
            _isAllowEmpty = isAllowEmpty;
        }
        public string GetDbFieldName()
        {
            return _dbFieldName;
        }
        public bool GetIsCondition()
        {
            return _isCondition;
        }
        public bool GetIsAllowEmpty()
        {
            return _isAllowEmpty;
        }
    }
}
