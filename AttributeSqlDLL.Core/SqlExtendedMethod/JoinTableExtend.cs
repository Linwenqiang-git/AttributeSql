using System.Text;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Core.Model;
using AttributeSqlDLL.Core.SqlAttribute.JoinTable;

namespace AttributeSqlDLL.Core.SqlExtendedMethod
{
    internal static class JoinTableExtend
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
            object[] Mainobj = typeof(T).GetCustomAttributes(typeof(MainTableAttribute), true);
            if (Mainobj == null || Mainobj.Length != 1)
            {
                throw new AttrSqlException("未定义主表或定义多个主表，请检查Dto特性配置!");
            }
            MainTableAttribute mainTable = Mainobj[0] as MainTableAttribute;
            join.Append($"FROM {mainTable.GetMainTableName()} {mainTable.GetMainTableByName()} ");
            object[] AllTableObj = typeof(T).GetCustomAttributes(true);
            foreach (var table in AllTableObj)
            {
                if (table.GetType() == typeof(LeftTableAttribute))
                {
                    LeftTableAttribute leftTable = table as LeftTableAttribute;
                    join.Append($"LEFT JOIN {leftTable.GetLeftTableName()} {leftTable.GetLeftTableByName()} ON {leftTable.GetConnectField()} ");
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
                    join.Append($"RIGHT JOIN {rightTable.GetRightTableName()} {rightTable.GetRightTableByName()} ON {rightTable.GetConnectField()} ");
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
                    join.Append($"INNER JOIN {innerTable.GetInnerTableName()} {innerTable.GetInnerTableByName()} ON {innerTable.GetConnectField()}");
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
                    join.Append($"{IncidenceRelation} JOIN ({suiblist.GetTableSql()}) {suiblist.GetInnerTableByName()} ON {suiblist.GetConnectField()}");
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
