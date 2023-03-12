using System;
using System.Text;

using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.CudAttr;

namespace AttributeSql.Core.SqlAttributeExtensions
{
    internal static class InsertExtension
    {
        internal static string InsertEntity(this AttrEntityBase entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"{SqlKeyWordEnum.Insert_Into.GetDescription()} {entity.GetType().Name}");
            sql.Append(SymbolEnum.LeftBrackets.GetDescription());
            foreach (var prop in entity.GetType().GetProperties())
            {
                sql.Append($"`{prop.Name}`,");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(SymbolEnum.RightBrackets.GetDescription());
            sql.Append(SqlKeyWordEnum.Values.GetDescription());
            sql.Append(SymbolEnum.LeftBrackets.GetDescription());
            foreach (var prop in entity.GetType().GetProperties())
            {
                sql.Append($"@{prop.Name},");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(SymbolEnum.RightBrackets.GetDescription());
            return sql.ToString();
        }
        /// <summary>
        /// 批量添加实体sql
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        internal static string BatchInsertEntity(this AttrEntityBase[] entities)
        {
            StringBuilder insertSql = new StringBuilder();
            insertSql.Append($"{SqlKeyWordEnum.Insert_Into.GetDescription()} {entities[0].GetType().Name}");
            insertSql.Append(SymbolEnum.LeftBrackets.GetDescription());
            foreach (var prop in entities[0].GetType().GetProperties())
            {
                //todo：根据不同的字段情况进行修改
                if (prop.Name.ToLower().Contains("modifyid") || 
                    prop.Name.ToLower().Contains("modifyby") || 
                    prop.Name.ToLower().Contains("modifytime") ||
                    prop.Name.ToLower().Contains("deleteid") ||
                    prop.Name.ToLower().Contains("deleteby") ||
                    prop.Name.ToLower().Contains("deletetime"))
                    continue;
                insertSql.Append($"`{prop.Name}`,");
            }
            insertSql.Remove(insertSql.Length - 1, 1);
            insertSql.Append(SymbolEnum.RightBrackets.GetDescription());
            insertSql.Append(SqlKeyWordEnum.Values.GetDescription());
            
            for (int i = 0; i < entities.Length; i++)
            {
                insertSql.Append(SymbolEnum.LeftBrackets.GetDescription());
                foreach (var prop in entities[0].GetType().GetProperties())
                {
                    if (prop.Name.ToLower().Contains("modifyid") ||
                        prop.Name.ToLower().Contains("modifyby") ||
                        prop.Name.ToLower().Contains("modifytime") ||
                        prop.Name.ToLower().Contains("deleteid") ||
                        prop.Name.ToLower().Contains("deleteby") ||
                        prop.Name.ToLower().Contains("deletetime"))
                        continue;
                    var type = prop.PropertyType.Name;
                    var value = prop.GetValue(entities[i], null);
                    if (type.ToLower() == "string" || type.ToLower() == "datetime")
                    {
                        if (value == null)
                            insertSql.Append($"{SqlKeyWordEnum.Null.GetDescription()},");
                        else
                            insertSql.Append($"'{value}',");
                    }
                    else
                    {
                        if (value == null)
                            insertSql.Append($"{SqlKeyWordEnum.Null.GetDescription()},");
                        else
                        {
                            //时间类型的要单独处理
                            if (prop.PropertyType.FullName.ToLower().Contains("datetime"))
                                insertSql.Append($"'{value}',");
                            else
                                insertSql.Append($"{value},");
                        }

                    }
                }
                insertSql.Remove(insertSql.Length - 1, 1);
                insertSql.Append(SymbolEnum.RightBrackets.GetDescription());
                if (i + 1 < entities.Length)
                {
                    insertSql.Append(",");
                }
            }                       
            return insertSql.ToString();
        }

        /// <summary>
        /// 新增Dto模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string InsertDtoModel(this AttrBaseModel model)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder values = new StringBuilder();
            object[] Mainobj = model.GetType().GetCustomAttributes(typeof(InsertTableAttribute), true);
            if (Mainobj == null || Mainobj.Length != 1)
            {
                throw new AttrSqlException("未定义新增表，请检查Dto特性配置!");
            }
            InsertTableAttribute mainTable = Mainobj[0] as InsertTableAttribute;
            sql.Append($"{SqlKeyWordEnum.Insert_Into.GetDescription()} {mainTable.GetInsertTableName()}");
            sql.Append(SymbolEnum.LeftBrackets.GetDescription());
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.IsDefined(typeof(DbFiledMappingAttribute), true))
                {
                    DbFiledMappingAttribute dbFiledMappingAttribute = prop.GetCustomAttributes(typeof(DbFiledMappingAttribute), true)[0] as DbFiledMappingAttribute;
                    sql.Append($"`{dbFiledMappingAttribute.GetDbFieldName()}`,");
                    values.Append($"@{dbFiledMappingAttribute.GetDbFieldName()},");
                }
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(SymbolEnum.RightBrackets.GetDescription());
            sql.Append(SqlKeyWordEnum.Values.GetDescription());
            sql.Append(SymbolEnum.LeftBrackets.GetDescription());
            sql.Append($"{values.ToString()}");
            sql.Remove(sql.Length - 1, 1);
            sql.Append(SymbolEnum.RightBrackets.GetDescription());
            return sql.ToString();
        }
    }
}
