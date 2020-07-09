using System;
using System.Text;
using AttributeSqlDLL.ExceptionExtension;
using AttributeSqlDLL.Model;

namespace AttributeSqlDLL.SqlExtendedMethod.CudExtend
{
    public static class DeleteExtend
    {
        /// <summary>
        /// 根据主键删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string DeleteByKey(this AttrEntityBase entity,string PrimaryFiled = null)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"Delete FROM {entity.GetType().Name} Where ");
            if (string.IsNullOrEmpty(PrimaryFiled))
            {
                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (string.IsNullOrEmpty(PrimaryFiled))
                    {
                        if (prop.Name.ToUpper() == "ID" || prop.Name.ToUpper().EndsWith("ID"))
                        {
                            PrimaryFiled = prop.Name;
                            //sql.Append($"{PrimaryFiled}=@{PrimaryFiled}");
                            break;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(PrimaryFiled))
                throw new AttrSqlException("删除实体失败:未找到主键字段，请配置要删除的主键字段");
            sql.Append($"{PrimaryFiled}=@{PrimaryFiled}");
            return sql.ToString();
        }
        /// <summary>
        /// 根据指定的字段删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string DeleteByCondition(this AttrEntityBase entity, string[] condition)
        {
            if (condition == null || condition.Length == 0)
                throw new AttrSqlException("未设置删除条件！");
            StringBuilder sql = new StringBuilder();
            sql.Append($"Delete FROM {entity.GetType().Name} Where ");
            for (int i = 0; i < condition.Length; i++)
            {
                sql.Append($"{condition[i]} = @{condition[i]}");
                if (i < condition.Length - 1)
                    sql.Append(" AND ");
            }
            return sql.ToString();
        }
        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="softDeleteField"></param>
        /// <param name="value"></param>
        /// <param name="IngnorIntDefault">忽略int类型的默认值0</param>
        /// <returns></returns>
        public static string SoftDeleteByCondition(this AttrEntityBase entity, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true)
        {
            if (string.IsNullOrEmpty(softDeleteField))
                throw new AttrSqlException("未设置软删除字段！");
            StringBuilder sql = new StringBuilder();
            sql.Append($"Update {entity.GetType().Name} SET {softDeleteField}={value}, ");
            string Primary = PrimaryKey;
            foreach (var prop in entity.GetType().GetProperties())
            {
                //存一下主键字段(默认名字为ID的或者以ID结尾的)
                if (string.IsNullOrEmpty(Primary))
                {
                    if (prop.Name.ToUpper() == "ID" || prop.Name.ToUpper().EndsWith("ID"))
                    {
                        Primary = prop.Name;
                        //主键字段不做更新，直接跳出
                        continue;
                    }
                }
                //软删除、主键字段也跳过，以参数为准
                if (prop.Name.ToUpper() == softDeleteField.ToUpper() || prop.Name.ToUpper() == PrimaryKey.ToUpper())
                {
                    continue;
                }
                //判断实体是否标记了AllowUpdateEmpty属性
                //if (prop.IsDefined(typeof(AllowUpdateEmptyAttribute), true))
                //{
                //    sql.Append($"{prop.Name} = @{prop.Name},");
                //    continue;
                //}
                bool isnull = false;
                //判断当前属性是否有值(主要针对string、int和数组类型)
                object objvalue = prop.GetValue(entity, null);
                if (objvalue == null)
                    continue;
                if (objvalue is string && string.IsNullOrEmpty((string)objvalue))
                    isnull = true;
                else if ((objvalue is int && (int)objvalue == 0) || (objvalue is long && (long)objvalue == 0) || (objvalue is byte && (byte)objvalue == 0))
                {
                    if (IngnorIntDefault)
                        isnull = true;
                }
                else if (objvalue is DateTime && (DateTime)objvalue == default(DateTime))
                {
                    isnull = true;
                }
                else if (objvalue is Array)
                {
                    Array array = (Array)objvalue;
                    if (array == null || array.Length == 0)
                        isnull = true;
                }
                //对有值的进行操作
                if (!isnull)
                {
                    sql.Append($"{prop.Name} = @{prop.Name},");
                }
            }
            if (sql.ToString() == $"Update {entity.GetType().Name} SET ")
            {
                return string.Empty;
            }
            sql = sql.Remove(sql.Length - 1, 1);
            if (string.IsNullOrEmpty(Primary))
            {
                throw new AttrSqlException($"未找到{entity.GetType().Name}表包含ID的主键字段,更新失败！");
            }
            sql.Append($" Where {Primary} = @{Primary}");
            return sql.ToString();
        }
    }
}
