﻿using System.Text;

using AttributeSql.Base.Enums;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.GroupHaving;

namespace AttributeSql.Core.SqlAttributeExtensions.QueryExtensions
{
    internal static class GroupByHavingExtension
    {
        internal static string GroupByHaving(this AttrBaseResult model)
        {
            StringBuilder groupbyBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();
            //拿到所有标记了该特性的字段
            groupbyBuilder.Append($" {SqlKeyWordEnum.Group_By.GetDescription()} ");
            havingBuilder.Append($" {SqlKeyWordEnum.Having.GetDescription()} ");
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.IsDefined(typeof(GroupByAttribute), true))
                {
                    GroupByAttribute groupBy = prop.GetCustomAttributes(typeof(GroupByAttribute), true)[0] as GroupByAttribute;
                    groupbyBuilder.Append($"{groupBy.GetGroupByField()},");
                }
                if (prop.IsDefined(typeof(HavingAttribute), true))
                {
                    HavingAttribute having = prop.GetCustomAttributes(typeof(HavingAttribute), true)[0] as HavingAttribute;
                    if (!string.IsNullOrEmpty(having.GetHavingCondition()))
                        havingBuilder.Append($" {having.GetHavingCondition()} {RelationEume.And.GetDescription()}");
                }
            }
            if (groupbyBuilder.ToString() == $" {SqlKeyWordEnum.Group_By.GetDescription()} ")
            {
                groupbyBuilder.Clear();
                return string.Empty;
            }
            else
                groupbyBuilder.Remove(groupbyBuilder.Length - 1, 1);
            if (havingBuilder.ToString() == $" {SqlKeyWordEnum.Having.GetDescription()} ")
            {
                havingBuilder.Clear();
            }
            else
            {
                havingBuilder.Remove(havingBuilder.Length - 3, 3);  //移除最后一个And
                groupbyBuilder.Append(havingBuilder.ToString());
            }
            //后面继续完善Having部分
            return groupbyBuilder.ToString();
        }
    }
}