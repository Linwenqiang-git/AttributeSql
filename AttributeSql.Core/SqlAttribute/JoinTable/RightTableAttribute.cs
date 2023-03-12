using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RightTableAttribute : Attribute
    {
        private string _rightTableName;
        private string _byName;
        private string _mainTableField;
        private string _joinField;
        private string _mainTableName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_RightTableName">右连接表名</param>
        /// <param name="_mainTableField">右表连接字段</param>
        /// <param name="_joinField">右连接表字段</param>
        /// <param name="_byName">右连接表别名</param>
        /// <param name="_mainTableName">连接主表别名(默认MainTable特性标记的为主表,也可以自己指定)</param>
        public RightTableAttribute(string rightTableName, string mainTableField, string joinField, string byName = "", string mainTableName = "")
        {
            _rightTableName = rightTableName;
            _byName = byName;
            _mainTableField = mainTableField;
            _joinField = joinField;
            _mainTableName = mainTableName;
        }
        public string GetRightTableName()
        {
            return _rightTableName;
        }
        public string GetRightTableByName()
        {
            return _byName;
        }
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
