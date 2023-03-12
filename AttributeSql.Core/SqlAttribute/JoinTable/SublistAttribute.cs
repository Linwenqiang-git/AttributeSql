using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    //子表的查询
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SublistAttribute : Attribute
    {
        private string _sublistSql;
        private string _incidenceRelation;
        private string _byName;
        private string _mainTableField;
        private string _joinField;
        private string _mainTableName;
        private List<string> IncidenceRelationCollection = new List<string>() { "INNER", "OUT", "LEFT", "RIGHT" };
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sublist">子表查询sql</param>
        /// <param name="incidenceRelation">与关联表的关联关系（值：INNER,LEFT,RIGHT,OUT）</param>
        /// <param name="mainTableField">主表连接字段</param>
        /// <param name="joinField">内连接表字段</param>
        /// <param name="byName">内连接表别名</param>
        /// <param name="mainTableName">内连接主表别名</param>
        public SublistAttribute(string sublist, string incidenceRelation, string mainTableField, string joinField, string byName = "", string mainTableName = "")
        {
            _sublistSql = sublist;
            _incidenceRelation = incidenceRelation;
            _byName = byName;
            _mainTableField = mainTableField;
            _joinField = joinField;
            _mainTableName = mainTableName;
        }
        public string GetTableSql()
        {
            if (string.IsNullOrEmpty(_sublistSql))
                throw new AttrSqlException("连接表sql不能为空，请检查Dto特性配置");
            return _sublistSql;
        }
        public string GetInnerTableByName()
        {
            return _byName;
        }
        public string GetIncidenceRelation()
        {
            if (string.IsNullOrEmpty(_incidenceRelation))
                throw new AttrSqlException("关联关系不能为空，请检查Dto特性配置");
            if (!IncidenceRelationCollection.Contains(_incidenceRelation.ToUpper()))
                throw new AttrSqlException("关联关系填写错误，请检查Dto特性配置");
            return _incidenceRelation;
        }
        /// <summary>
        /// 获取子查询表连接字符串
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
                throw new Exception("主表连接字段不能为空，请检查Dto特性配置");
            return _mainTableField;
        }
        public string GetMainTableByName()
        {
            return _mainTableName;
        }
    }
}
