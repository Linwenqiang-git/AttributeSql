using System;
using System.Text;
using AttributeSqlDLL.Model;

namespace AttributeSqlDLL.SqlExtendedMethod.CudExtend
{
    public static class InsertExtend
    {
        public static string InsertEntity(this AttrEntityBase entity)
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
        public static string BatchInsertEntity(this AttrEntityBase[] entities)
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
    }
}
