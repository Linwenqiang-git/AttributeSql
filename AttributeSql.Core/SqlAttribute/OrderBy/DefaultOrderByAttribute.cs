using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.OrderBy
{
    /// <summary>
    /// 默认排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultOrderByAttribute : System.Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        private string tableName;
        /// <summary>
        /// 表别名
        /// </summary>
        private string byName;
        /// <summary>
        /// 排序字段
        /// </summary>
        private string sortField;
        /// <summary>
        /// 排序方式
        /// </summary>
        private string sortWay;
        /// <summary>
        /// 初始化，排序字段和排序方式必须要传
        /// </summary>
        /// <param name="_sortWay"></param>
        /// <param name="_sortField"></param>
        /// <param name="_tableName"></param>
        /// <param name="_byName"></param>
        public DefaultOrderByAttribute(string _sortField, string _sortWay, string _byName = "", string _tableName = "")
        {
            tableName = _tableName;
            byName = _byName;
            sortField = _sortField;
            sortWay = _sortWay;
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return tableName;
        }
        /// <summary>
        /// 别名
        /// </summary>
        /// <returns></returns>
        public string GetByName()
        {
            return byName;
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        /// <returns></returns>
        public string GetSortField()
        {
            return sortField;
        }
        /// <summary>
        /// 排序方式
        /// </summary>
        /// <returns></returns>
        public string GetSortWay()
        {
            return sortWay;
        }
    }
}
