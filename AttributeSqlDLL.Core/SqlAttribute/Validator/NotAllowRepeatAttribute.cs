using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.Validator
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NotAllowRepeatAttribute : Attribute
    {
        private string DbFieldName { get; set; }
        private string[] DbFieldNames { get; set; }
        private string TableName { get; set; }
        private string Key { get; set; }
        private string Msg { get; set; }
        private string SoftFieldName { get; set; }
        private long SoftFieldValue { get; set; }
        private bool IsRemoveSoftDeleteField { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_DbFieldName">字段名即不能重复的字段</param>
        /// <param name="_TableName">表名</param>
        /// <param name="_Key">表主键字段</param>
        /// <param name="_Msg">错误信息</param>
        /// <param name="_SoftFieldName">软删除字段名称</param>
        /// <param name="_SoftFieldValue">软删除标记的有效值</param>
        /// <param name="_IsRemoveSoftDeleteField">是否将软删除字段纳入删除条件,默认纳入</param>
        public NotAllowRepeatAttribute(string _DbFieldName, string _TableName, string _Key, string _Msg,string _SoftFieldName = "", long _SoftFieldValue = 0, bool _IsRemoveSoftDeleteField = false)
        {
            DbFieldName = _DbFieldName;
            TableName = _TableName;
            Key = _Key;
            Msg = _Msg;
            SoftFieldName = _SoftFieldName;
            IsRemoveSoftDeleteField = _IsRemoveSoftDeleteField;
            SoftFieldValue = _SoftFieldValue;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_DbFieldName">字段名即不能重复的字段</param>
        /// <param name="_TableName">表名</param>
        /// <param name="_Key">表主键字段</param>
        /// <param name="_Msg">错误信息</param>
        /// <param name="_SoftFieldName">软删除字段名称</param>
        /// <param name="_SoftFieldValue">软删除标记的有效值</param>
        /// <param name="_IsRemoveSoftDeleteField">是否将软删除字段纳入删除条件,默认纳入</param>
        public NotAllowRepeatAttribute(string[] _DbFieldName, string _TableName, string _Key, string _Msg, string _SoftFieldName = "", long _SoftFieldValue = 0, bool _IsRemoveSoftDeleteField = false)
        {
            DbFieldNames = _DbFieldName;
            TableName = _TableName;
            Key = _Key;
            Msg = _Msg;
            SoftFieldName = _SoftFieldName;
            IsRemoveSoftDeleteField = _IsRemoveSoftDeleteField;
            SoftFieldValue = _SoftFieldValue;
        }
        /// <summary>
        /// 获取表字段名
        /// </summary>
        /// <returns></returns>
        public string GetDbFieldName()
        {
            return DbFieldName;
        }
        /// <summary>
        /// 获取多字段的校验
        /// </summary>
        /// <returns></returns>
        public string[] GetDbFieldNames()
        {
            return DbFieldNames;
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return TableName;
        }
        /// <summary>
        /// 获取出错信息
        /// </summary>
        /// <returns></returns>
        public string GetMsg()
        {
            return Msg;
        }
        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryKey()
        {
            return Key;
        }
        /// <summary>
        /// 返回软删除字段名称
        /// </summary>
        /// <returns></returns>
        public string GetSoftDeleteFieldName()
        {
            if (!IsRemoveSoftDeleteField)
                return SoftFieldName;
            return string.Empty;
        }
        /// <summary>
        /// 获取软删除标记字段值
        /// </summary>
        /// <returns></returns>
        public long GetSoftDeleteFieldValue()
        {
            return SoftFieldValue;
        }
    }
}
