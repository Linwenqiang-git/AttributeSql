using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Validator
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NotAllowRepeatAttribute : Attribute
    {
        private string _dbFieldName;
        private string[] _dbFieldNames;
        private string _tableName;
        private string _key;
        private string _msg;
        private string _softFieldName;
        private long _softFieldValue;
        private bool _isRemoveSoftDeleteField;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbFieldName">字段名即不能重复的字段</param>
        /// <param name="tableName">表名</param>
        /// <param name="key">表主键字段</param>
        /// <param name="msg">错误信息</param>
        /// <param name="softFieldName">软删除字段名称</param>
        /// <param name="softFieldValue">软删除标记的有效值</param>
        /// <param name="isRemoveSoftDeleteField">是否将软删除字段纳入删除条件,默认纳入</param>
        public NotAllowRepeatAttribute(string dbFieldName, string tableName, string key, string msg,string softFieldName = "", long softFieldValue = 0, bool isRemoveSoftDeleteField = false)
        {
            _dbFieldName = dbFieldName;
            _tableName = tableName;
            _key = key;
            _msg = msg;
            _softFieldName = softFieldName;
            _isRemoveSoftDeleteField = isRemoveSoftDeleteField;
            _softFieldValue = softFieldValue;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbFieldNames">字段名即不能重复的字段</param>
        /// <param name="tableName">表名</param>
        /// <param name="key">表主键字段</param>
        /// <param name="msg">错误信息</param>
        /// <param name="softFieldName">软删除字段名称</param>
        /// <param name="softFieldValue">软删除标记的有效值</param>
        /// <param name="isRemoveSoftDeleteField">是否将软删除字段纳入删除条件,默认纳入</param>
        public NotAllowRepeatAttribute(string[] dbFieldNames, string tableName, string key, string msg, string softFieldName = "", long softFieldValue = 0, bool isRemoveSoftDeleteField = false)
        {
            _dbFieldNames = dbFieldNames;
            _tableName = tableName;
            _key = key;
            _msg = msg;
            _softFieldName = softFieldName;
            _isRemoveSoftDeleteField = isRemoveSoftDeleteField;
            _softFieldValue = softFieldValue;
        }
        /// <summary>
        /// 获取表字段名
        /// </summary>
        /// <returns></returns>
        public string GetDbFieldName()
        {
            return _dbFieldName;
        }
        /// <summary>
        /// 获取多字段的校验
        /// </summary>
        /// <returns></returns>
        public string[] GetDbFieldNames()
        {
            return _dbFieldNames;
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return _tableName;
        }
        /// <summary>
        /// 获取出错信息
        /// </summary>
        /// <returns></returns>
        public string GetMsg()
        {
            return _msg;
        }
        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryKey()
        {
            return _key;
        }
        /// <summary>
        /// 返回软删除字段名称
        /// </summary>
        /// <returns></returns>
        public string GetSoftDeleteFieldName()
        {
            if (!_isRemoveSoftDeleteField)
                return _softFieldName;
            return string.Empty;
        }
        /// <summary>
        /// 获取软删除标记字段值
        /// </summary>
        /// <returns></returns>
        public long GetSoftDeleteFieldValue()
        {
            return _softFieldValue;
        }
    }
}
