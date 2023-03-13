using AttributeSql.Base.Exceptions;
using AttributeSql.Core.Data;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.OrderBy;
using AttributeSql.Core.SqlAttribute.Select;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.SqlAttributeExtensions
{
    internal static class AttrPageSearchExtension
    {
        /// <summary>
        /// 时间字段搜索扩展为一天
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public static TPageSearch TimeConvert<TPageSearch>(this TPageSearch pageSearch) where TPageSearch : AttrPageSearch
        {
            if (pageSearch == null)
                return default;
            foreach (var prop in pageSearch.GetType().GetProperties())
            {
                if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                {
                    DbFieldNameAttribute? fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                    //时间类型字段
                    if (fieldName != null && fieldName.DatetimeField())
                    {
                        //判断当前属性是否有值(主要针对string)
                        object? objvalue = prop?.GetValue(pageSearch, null);
                        if (objvalue != null && objvalue is string && !string.IsNullOrEmpty((string)objvalue))
                        {
                            string TimeSuffix = fieldName.GetTimeSuffix();
                            if (string.IsNullOrEmpty(TimeSuffix))
                            {
                                if (prop.Name.ToLower().Contains("start"))
                                    TimeSuffix = " 00:00:00";
                                else if (prop.Name.ToLower().Contains("end"))
                                    TimeSuffix = " 23:59:59";
                            }
                            prop.SetValue(pageSearch, (string)objvalue + TimeSuffix);
                        }
                    }
                }
            }
            return pageSearch;
        }
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
        /// <summary>
        /// 数据过滤器
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <param name="abpDataFilter"></param>
        /// <returns></returns>
        internal static void AbpDataFilter(this AttrPageSearch pageSearch, IAttrSqlWithAbpDataFilter abpDataFilter)
        {
            pageSearch.Tenantid = abpDataFilter.GetCurrentTenantId();
            bool? isFilterDelete = abpDataFilter.IsFilterDelete();
            if (isFilterDelete != null && (bool)isFilterDelete)
                pageSearch.Isdeleted = false;
        }
    }
}
