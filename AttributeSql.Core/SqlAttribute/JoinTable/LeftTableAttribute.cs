using AttributeSql.Base.Exceptions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LeftTableAttribute : Attribute
    {
        private string _leftTableName;
        private string _byName;
        private string _mainTableField;
        private string _joinField;
        private string _mainTableName;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="leftTableName">左连接表名</param>
        /// <param name="mainTableField">主表连接字段</param>
        /// <param name="joinField">左连接表字段</param>
        /// <param name="byName">左连接表别名</param>
        /// <param name="mainTableName">连接主表别名(默认MainTable特性标记的为主表,也可以自己指定)</param>
        public LeftTableAttribute(string leftTableName, string mainTableField, string joinField, string byName = "", string mainTableName = "")
        {
            _leftTableName = leftTableName;
            _byName = byName;
            _mainTableField = mainTableField;
            _joinField = joinField;
            _mainTableName = mainTableName;
        }
        public string GetLeftTableName()
        {
            return _leftTableName;
        }
        public string GetLeftTableByName()
        {
            return _byName;
        }
        /// <summary>
        /// 获取左表连接字符串
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
            join.Append("=");
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
        /// <summary>
        /// 返回连接主表别名
        /// </summary>
        /// <returns></returns>
        public string GetMainTableByName()
        {
            return _mainTableName;
        }
    }
}
