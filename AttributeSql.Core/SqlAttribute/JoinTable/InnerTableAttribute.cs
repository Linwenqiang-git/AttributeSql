using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InnerTableAttribute : Attribute
    {
        private string _innerTableName;
        private string _byName;
        private string _mainTableField;
        private string _joinField;
        private string _mainTableName;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="innerTableName">内连接表名</param主
        /// <param name="mainTableField">主表连接字段</param>
        /// <param name="joinField">内连接表字段</param>
        /// <param name="byName">内连接表别名</param>
        /// <param name="mainTableName">内连接住表别名</param>
        public InnerTableAttribute(string innerTableName, string mainTableField, string joinField, string byName = "", string mainTableName = "")
        {
            _innerTableName = innerTableName;
            _byName = byName;
            _mainTableField = mainTableField;
            _joinField = joinField;
            _mainTableName = mainTableName;
        }
        public string GetInnerTableName()
        {
            if (string.IsNullOrEmpty(_innerTableName))
                throw new AttrSqlException("内连接表不能为空，请检查Dto特性配置");
            return _innerTableName;
        }
        public string GetInnerTableByName()
        {
            return _byName;
        }
        /// <summary>
        /// 获取内表连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectField()
        {
            if (string.IsNullOrEmpty(_mainTableField) || string.IsNullOrEmpty(_joinField))
                throw new AttrSqlException("表连接字段不能为空，请检查Dto特性配置");
            StringBuilder join = new StringBuilder();
            if (!string.IsNullOrEmpty(_byName))
            {
                join.Append($"{_byName}.{_joinField}");
            }
            else
            {
                join.Append($"{_joinField}");
            }
            join.Append(OperatorEnum.Equal.GetDescription());
            return join.ToString();
        }
        /// <summary>
        /// 获取主表连接字段
        /// </summary>
        /// <returns></returns>
        public string GetMainTableField()
        {
            if (string.IsNullOrEmpty(_mainTableField))
                throw new AttrSqlException("主表连接字段不能为空，请检查Dto特性配置");
            return _mainTableField;
        }
        public string GetMainTableByName()
        {
            return _mainTableName;
        }
    }
}
