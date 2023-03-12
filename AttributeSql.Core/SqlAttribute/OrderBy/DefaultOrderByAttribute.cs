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
        private string _tableName;
        /// <summary>
        /// 表别名
        /// </summary>
        private string _byName;
        /// <summary>
        /// 排序字段
        /// </summary>
        private string _sortField;
        /// <summary>
        /// 排序方式
        /// </summary>
        private string _sortWay;
        /// <summary>
        /// 初始化，排序字段和排序方式必须要传
        /// </summary>
        /// <param name="sortWay"></param>
        /// <param name="sortField"></param>
        /// <param name="tableName"></param>
        /// <param name="byName"></param>
        public DefaultOrderByAttribute(string sortField, string sortWay, string byName = "", string tableName = "")
        {
            _tableName = tableName;
            _byName = byName;
            _sortField = sortField;
            _sortWay = sortWay;
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
        /// 别名
        /// </summary>
        /// <returns></returns>
        public string GetByName()
        {
            return _byName;
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        /// <returns></returns>
        public string GetSortField()
        {
            return _sortField;
        }
        /// <summary>
        /// 排序方式
        /// </summary>
        /// <returns></returns>
        public string GetSortWay()
        {
            return _sortWay;
        }
    }
}
