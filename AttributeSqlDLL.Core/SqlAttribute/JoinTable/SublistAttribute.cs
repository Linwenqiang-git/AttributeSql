using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.JoinTable
{
    //子表的查询
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SublistAttribute : Attribute
    {
        private string SublistSql;
        private string IncidenceRelation;
        private string byName;
        private string mainTableField;
        private string joinField;
        private string mainTableName;
        private string[] otherJoinsWhere;
        private List<string> IncidenceRelationCollection = new List<string>() { "INNER", "OUT", "LEFT", "RIGHT" };
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_Sublist">子表查询sql</param>
        /// <param name="_IncidenceRelation">与关联表的关联关系（值：INNER,LEFT,RIGHT,OUT）</param>
        /// <param name="_mainTableField">主表连接字段</param>
        /// <param name="_joinField">内连接表字段</param>
        /// <param name="_byName">内连接表别名</param>
        /// <param name="_mainTableName">内连接主表别名</param>
        /// <param name="_otherJoinsWhere">其他连接条件，表别名需指定：A.a = B.a </param>
        public SublistAttribute(string _Sublist, string _IncidenceRelation, string _mainTableField, string _joinField, string _byName = "", string _mainTableName = "", string[] _otherJoinsWhere = null)
        {
            SublistSql = _Sublist;
            IncidenceRelation = _IncidenceRelation;
            byName = _byName;
            mainTableField = _mainTableField;
            joinField = _joinField;
            mainTableName = _mainTableName;
            otherJoinsWhere = _otherJoinsWhere;
        }
        public string GetTableSql()
        {
            if (string.IsNullOrEmpty(SublistSql))
                throw new Exception("连接表sql不能为空，请检查Dto特性配置");
            return SublistSql;
        }
        public string GetInnerTableByName()
        {
            return byName;
        }
        public string GetIncidenceRelation()
        {
            if (string.IsNullOrEmpty(IncidenceRelation))
                throw new Exception("关联关系不能为空，请检查Dto特性配置");
            if (!IncidenceRelationCollection.Contains(IncidenceRelation.ToUpper()))
                throw new Exception("关联关系填写错误，请检查Dto特性配置");
            return IncidenceRelation;
        }
        /// <summary>
        /// 获取子查询表连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectField()
        {
            if (string.IsNullOrEmpty(mainTableField) || string.IsNullOrEmpty(joinField))
                throw new Exception("表连接字段不能为空，请检查Dto特性配置");
            StringBuilder join = new StringBuilder();
            if (!string.IsNullOrEmpty(byName))
            {
                join.Append($"{byName}.{joinField}");
            }
            else
            {
                join.Append($"{joinField}");
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
            if (string.IsNullOrEmpty(mainTableField))
                throw new Exception("主表连接字段不能为空，请检查Dto特性配置");
            return mainTableField;
        }
        public string GetMainTableByName()
        {
            return mainTableName;
        }
        /// <summary>
        /// 返回其他连接条件
        /// </summary>
        /// <returns></returns>
        public string[] GetOtherJoinsWhere()
        {
            return otherJoinsWhere;
        }
    }
}
