using System;
using System.Text;
using AttributeSqlDLL.ExceptionExtension;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.OrderBy;

namespace AttributeSqlDLL.SqlExtendedMethod
{
    internal static class DefaultOrderByExtend
    {
        /// <summary>
        /// 获取该查询默认的排序方法
        /// 目前只支持单一默认字段的排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string DefaultSort(this AttrPageSearch model)
        {
            object[] sort = model.GetType().GetCustomAttributes(typeof(DefaultOrderByAttribute), true);
            if (sort?.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder orderby = new StringBuilder();
            orderby.Append($"ORDER BY ");
            for (int i = 0; i < sort?.Length; i++)
            {
                DefaultOrderByAttribute defaultOrderBy = sort[i] as DefaultOrderByAttribute;
                if (string.IsNullOrEmpty(defaultOrderBy.GetSortWay()))
                {
                    throw new AttrSqlException("未定义排序方式，请检查PageSearch特性配置!");
                }
                if (defaultOrderBy.GetSortWay().ToUpper() != "ASC" && defaultOrderBy.GetSortWay().ToUpper() != "DESC")
                {
                    throw new AttrSqlException("排序方式定义错误，请检查PageSearch特性配置!");
                }
                if (string.IsNullOrEmpty(defaultOrderBy.GetSortField()))
                {
                    throw new AttrSqlException("未定义排序字段，请检查PageSearch特性配置!");
                }
                if (!string.IsNullOrEmpty(defaultOrderBy.GetByName()))
                {
                    orderby.Append($" {defaultOrderBy.GetByName()}.");
                }
                orderby.Append($"{defaultOrderBy.GetSortField()} {defaultOrderBy.GetSortWay()}");
                orderby.Append($",");
            }
            orderby = orderby.Remove(orderby.Length - 1, 1);
            orderby.Append(" ");
            if (orderby.ToString() == "ORDER BY ")
            {
                return string.Empty;    
            }
            return orderby.ToString();
        }
    }
}
