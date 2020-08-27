using System;
using System.Text;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Core.Model;
using AttributeSqlDLL.Core.SqlAttribute.CudAttr;

namespace AttributeSqlDLL.Core.SqlExtendedMethod.CudExtend
{
    internal static class InsertExtend
    {
        internal static string InsertEntity(this AttrEntityBase entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"INSERT INTO {entity.GetType().Name}");
            sql.Append("(");
            foreach (var prop in entity.GetType().GetProperties())
            {
                sql.Append($"`{prop.Name}`,");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            sql.Append("VALUES");
            sql.Append("(");
            foreach (var prop in entity.GetType().GetProperties())
            {
                sql.Append($"@{prop.Name},");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
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
            insertSql.Append($"INSERT INTO {entities[0].GetType().Name}");
            insertSql.Append("(");
            foreach (var prop in entities[0].GetType().GetProperties())
            {
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
            insertSql.Append(")");
            insertSql.Append("VALUES");
            
            for (int i = 0; i < entities.Length; i++)
            {
                insertSql.Append("(");
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
                            insertSql.Append($"NULL,");
                        else
                            insertSql.Append($"'{value}',");
                    }
                    else
                    {
                        if (value == null)
                            value = "null";
                        insertSql.Append($"{value},");
                    }
                }
                insertSql.Remove(insertSql.Length - 1, 1);
                insertSql.Append(")");
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
            sql.Append($"INSERT INTO {mainTable.GetInsertTableName()}");
            sql.Append("(");
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
            sql.Append(")");
            sql.Append("VALUES");
            sql.Append("(");
            sql.Append($"{values.ToString()}");
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            return sql.ToString();
        }
    }
}
