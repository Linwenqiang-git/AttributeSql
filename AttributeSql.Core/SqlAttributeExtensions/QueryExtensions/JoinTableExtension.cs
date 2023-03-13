using System.Text;

using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.JoinTable;

namespace AttributeSql.Core.SqlAttributeExtensions.QueryExtensions
{
    internal static class JoinTableExtension
    {
        /// <summary>
        /// 获取连接的表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string Join<T>(this AttrBaseResult model) where T : AttrBaseResult
        {
            StringBuilder join = new StringBuilder();
            object[] mainObj = typeof(T).GetCustomAttributes(typeof(MainTableAttribute), true);
            if (mainObj == null || mainObj.Length != 1)
            {
                throw new AttrSqlException("未定义主表或定义多个主表，请检查Dto特性配置!");
            }
            MainTableAttribute mainTable = mainObj[0] as MainTableAttribute;
            join.Append($"{SqlKeyWordEnum.From.GetDescription()} {mainTable.GetMainTableName()} {mainTable.GetMainTableByName()} ");
            object[] allTableObj = typeof(T).GetCustomAttributes(true);
            foreach (var table in allTableObj)
            {
                if (table.GetType() == typeof(LeftTableAttribute))
                {
                    LeftTableAttribute leftTable = table as LeftTableAttribute;
                    join.Append($"{SqlKeyWordEnum.Left_Join.GetDescription()} " +
                                $"{leftTable.GetLeftTableName()} {leftTable.GetLeftTableByName()} " +
                                $"{SqlKeyWordEnum.On.GetDescription()} {leftTable.GetConnectField()} ");
                    if (!string.IsNullOrEmpty(leftTable.GetMainTableByName()))
                    {
                        join.Append($"{leftTable.GetMainTableByName()}.{leftTable.GetMainTableField()} ");
                    }
                    else if (!string.IsNullOrEmpty(mainTable.GetMainTableByName()))
                    {
                        join.Append($"{mainTable.GetMainTableByName()}.{leftTable.GetMainTableField()} ");
                    }
                    else
                    {
                        join.Append($"{leftTable.GetMainTableField()} ");
                    }
                }
                else if (table.GetType() == typeof(RightTableAttribute))
                {
                    RightTableAttribute rightTable = table as RightTableAttribute;
                    join.Append($"{SqlKeyWordEnum.Right_Join.GetDescription()} {rightTable.GetRightTableName()} {rightTable.GetRightTableByName()} {SqlKeyWordEnum.On.GetDescription()} {rightTable.GetConnectField()} ");
                    if (!string.IsNullOrEmpty(rightTable.GetMainTableByName()))
                    {
                        join.Append($"{rightTable.GetMainTableByName()}.{rightTable.GetMainTableField()} ");
                    }
                    else if (!string.IsNullOrEmpty(mainTable.GetMainTableByName()))
                    {
                        join.Append($"{mainTable.GetMainTableByName()}.{rightTable.GetMainTableField()} ");
                    }
                    else
                    {
                        join.Append($"{rightTable.GetMainTableField()} ");
                    }
                }
                else if (table.GetType() == typeof(InnerTableAttribute))
                {
                    InnerTableAttribute innerTable = table as InnerTableAttribute;
                    join.Append($"{SqlKeyWordEnum.Inner_Join.GetDescription()} {innerTable.GetInnerTableName()} {innerTable.GetInnerTableByName()} {SqlKeyWordEnum.On.GetDescription()} {innerTable.GetConnectField()}");
                    if (!string.IsNullOrEmpty(innerTable.GetMainTableByName()))
                    {
                        join.Append($"{innerTable.GetMainTableByName()}.{innerTable.GetMainTableField()} ");
                    }
                    else if (!string.IsNullOrEmpty(mainTable.GetMainTableByName()))
                    {
                        join.Append($"{mainTable.GetMainTableByName()}.{innerTable.GetMainTableField()} ");
                    }
                    else
                    {
                        join.Append($"{innerTable.GetMainTableField()} ");
                    }
                }
                else if (table.GetType() == typeof(SublistAttribute))
                {
                    SublistAttribute suiblist = table as SublistAttribute;
                    string IncidenceRelation = suiblist.GetIncidenceRelation();
                    join.Append($"{IncidenceRelation} {SqlKeyWordEnum.Join.GetDescription()} ({suiblist.GetTableSql()}) {suiblist.GetInnerTableByName()} {SqlKeyWordEnum.On.GetDescription()} {suiblist.GetConnectField()}");
                    if (!string.IsNullOrEmpty(suiblist.GetMainTableByName()))
                    {
                        join.Append($"{suiblist.GetMainTableByName()}.{suiblist.GetMainTableField()} ");
                    }
                    else if (!string.IsNullOrEmpty(mainTable.GetMainTableByName()))
                    {
                        join.Append($"{mainTable.GetMainTableByName()}.{suiblist.GetMainTableField()} ");
                    }
                    else
                    {
                        join.Append($"{suiblist.GetMainTableField()} ");
                    }
                }
            }
            return join.ToString();

        }
    }
}
