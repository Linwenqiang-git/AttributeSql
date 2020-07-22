using System;
using System.Text;
using AttributeSqlDLL.Core.ExceptionExtension;
using AttributeSqlDLL.Core.Model;
using AttributeSqlDLL.Core.SqlAttribute.JoinTable;
using AttributeSqlDLL.Core.SqlAttribute.Select;
using AttributeSqlDLL.Core.SqlAttribute.Where;

namespace AttrSqlDbLite.Core.SqlExtendedMethod
{
    internal static class WhereConditionExtend
    {
        #region 参数化where条件
        /// <summary>
        /// 参数化where条件,只针对标记特性且有值的字段生效
        /// 特性解析规则按照 [聚合函数]([表别名.]{字段名}) {操作符} {参数化变量} [非聚合函数]
        /// 参数化字段的名称需要与模型字段名称一致
        /// </summary>
        /// <param name="model"></param>
        /// <param name="IngnorIntDefault">int类型的默认值是否忽略,默认忽略</param>
        /// <returns></returns>
        internal static string ParaWhere(this AttrPageSearch model, bool IngnorIntDefault = true)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE 1=1");
            foreach (var prop in model.GetType().GetProperties())
            {
                //对没有标记操作特性的字段不操作
                if (prop.GetCustomAttributes(typeof(OperationCodeAttribute),true).Length == 0 &&
                    prop.GetCustomAttributes(typeof(OrOperationAttribute), true).Length == 0)
                {
                    continue;
                }
                bool isnull = false;
                //判断当前属性是否有值(主要针对string、int和数组类型)
                object objvalue = prop.GetValue(model, null);
                if (objvalue == null)
                    continue;
                if (objvalue is string && string.IsNullOrEmpty((string)objvalue))
                    isnull = true;
                else if (objvalue is int && (int)objvalue == 0)
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
                    bool isAppend = false;
                    builder.Append(" AND ");
                    string fieldCondition = prop.Name;
                    //表名.字段
                    if (prop.IsDefined(typeof(TableByNameAttribute), true))
                    {
                        TableByNameAttribute byName = prop.GetCustomAttributes(typeof(TableByNameAttribute), true)[0] as TableByNameAttribute;
                        fieldCondition = $" {byName.GetName()}.";
                        if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                        {
                            DbFieldNameAttribute fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                            fieldCondition += fieldName.GetDbFieldName();
                        }
                        else
                            fieldCondition += $"{prop.Name}";
                        isAppend = true;
                    }
                    else if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                    {
                        DbFieldNameAttribute fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                        fieldCondition = fieldName.GetDbFieldName();
                        isAppend = true;
                    }
                    //添加聚合函数
                    if (prop.IsDefined(typeof(AggregateFuncAttribute), true))
                    {
                        AggregateFuncAttribute aggregateFunc = prop.GetCustomAttributes(typeof(AggregateFuncAttribute), true)[0] as AggregateFuncAttribute;
                        fieldCondition += aggregateFunc.GetFuncoperate() + $"({fieldCondition})";
                        isAppend = true;
                    }
                    //操作符 [like | in  |  =  | is | find_in_set]
                    if (prop.IsDefined(typeof(OperationCodeAttribute), true))
                    {
                        builder.Append($" {fieldCondition}"); //条件字段名
                        OperationCodeAttribute option = prop.GetCustomAttributes(typeof(OperationCodeAttribute), true)[0] as OperationCodeAttribute;
                        builder.Append($" {option.GetOption()}");//操作符
                        if (option.GetOption().ToUpper().Trim().Contains("IS"))
                        {
                            //NULL 标记的操作符，字段值为1则为IS NOT NULL 2为 IS NULL 
                            int value = (int)objvalue;
                            if (value == 1)
                            {
                                builder.Append($" NOT NULL ");
                            }
                            else if (value == 2)
                            {
                                builder.Append($" NULL ");
                            }
                            else
                            {
                                throw new AttrSqlException("IS 操作符的值只允许为1(NOT NULL)或2(NULL),请检查传入参数的值是否正确！");
                            }
                        }
                        else if (option.GetOption().ToUpper().Trim().Contains("LIKE"))
                        {
                            if (objvalue is string)
                            {
                                builder.Append($" '%{((string)objvalue).Trim().Replace("--","")}%'");
                            }
                            else
                                builder.Append($" @{prop.Name}");//参数化查询
                        }
                        else if (option.GetOption().ToUpper().Trim().Contains("IN"))
                        {
                            builder.Remove(builder.Length - 2, 2);//先去掉操作符
                            //+1 加的是空格
                            builder.Remove(builder.Length - (fieldCondition.Length + 1), fieldCondition.Length + 1);
                            builder.Append("FIND_IN_SET");
                            builder.Append($"({fieldCondition},@{prop.Name})");
                        }
                        else
                            builder.Append($" @{prop.Name}");//参数化查询
                        isAppend = true;
                    }
                    else if (prop.IsDefined(typeof(OrOperationAttribute), true))
                    {
                        //定义OR条件连接,一般存在OR连接的字段就不允许出现其他操作符
                        OrOperationAttribute orOperationAttribute = prop.GetCustomAttributes(typeof(OrOperationAttribute), true)[0] as OrOperationAttribute;
                        string sql = orOperationAttribute.GetOrOperation();
                        if (sql.ToUpper().Contains("LIKE"))
                        {
                            if (objvalue is string)
                            {
                                sql = sql.Replace($"@{prop.Name}", $"{((string)objvalue).Trim()}");
                            }
                        }
                        builder.Append(sql);
                        continue;
                    }
                    //非聚合函数
                    if (prop.IsDefined(typeof(NonAggregateFuncAttribute), true))
                    {
                        if (isAppend)
                            builder.Append(" AND ");
                        NonAggregateFuncAttribute aggregateFunc = prop.GetCustomAttributes(typeof(NonAggregateFuncAttribute), true)[0] as NonAggregateFuncAttribute;
                        fieldCondition = aggregateFunc.GetFuncoperate();
                        builder.Append(fieldCondition);
                    }

                }
            }
            if (builder.ToString() == " WHERE 1=1")
                return "";
            return builder.ToString();//去掉最后一个逗号
        }
        /// <summary>
        /// 自行拼装额外的where条件
        /// </summary>
        /// <param name="paraWhere"></param>
        /// <param name="extraWhere"></param>
        /// <returns></returns>
        internal static string ExtraWhere(this string paraWhere, string extraWhere)
        {
            return paraWhere + extraWhere;
        }
        /// <summary>
        /// 根据条件额外拼接where条件
        /// </summary>
        /// <param name="paraWhere"></param>
        /// <param name="func"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string ExtraWhere(this string paraWhere, Func<AttrPageSearch, string> func, AttrPageSearch model)
        {
            string ExtraWhere = func.Invoke(model) != string.Empty ? $"  AND {func.Invoke(model)}" : func.Invoke(model);
            return paraWhere + ExtraWhere;
        }
        #endregion

        #region 非参数化where条件
        /// <summary>
        /// 非参数化where条件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string NonParaWhere(this AttrPageSearch model)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE 1=1");
            foreach (var prop in model.GetType().GetProperties())
            {
                //对没有标记特性的字段不操作
                if (prop.GetCustomAttributes(true).Length == 0)
                {
                    continue;
                }
                bool isnull = false;
                //判断当前属性是否有值
                object objvalue = prop.GetValue(model, null);
                if (objvalue is string && string.IsNullOrEmpty((string)objvalue))
                    isnull = true;
                else if (objvalue is Array)
                {
                    Array array = (Array)objvalue;
                    if (array == null || array.Length == 0)
                        isnull = true;
                }
                //对有值的进行操作
                if (!isnull)
                {
                    bool isAppend = false;
                    builder.Append(" AND ");
                    string fieldCondition = prop.Name;
                    //表名.字段
                    if (prop.IsDefined(typeof(TableByNameAttribute), true))
                    {
                        TableByNameAttribute byName = prop.GetCustomAttributes(typeof(TableByNameAttribute), true)[0] as TableByNameAttribute;
                        fieldCondition = $" {byName.GetName()}.";
                        if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                        {
                            DbFieldNameAttribute fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                            fieldCondition += fieldName.GetDbFieldName();
                        }
                        else
                            fieldCondition += $" {prop.Name}";
                        isAppend = true;

                    }
                    else if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                    {
                        DbFieldNameAttribute fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                        fieldCondition = fieldName.GetDbFieldName();
                        isAppend = true;
                    }
                    //添加聚合函数
                    if (prop.IsDefined(typeof(AggregateFuncAttribute), true))
                    {
                        AggregateFuncAttribute aggregateFunc = prop.GetCustomAttributes(typeof(AggregateFuncAttribute), true)[0] as AggregateFuncAttribute;
                        fieldCondition += aggregateFunc.GetFuncoperate() + $"({fieldCondition})";
                        isAppend = true;
                    }
                    //操作符 like in =
                    if (prop.IsDefined(typeof(OperationCodeAttribute), true))
                    {
                        builder.Append($" {fieldCondition}");
                        OperationCodeAttribute option = prop.GetCustomAttributes(typeof(OperationCodeAttribute), true)[0] as OperationCodeAttribute;
                        builder.Append($" {option.GetOption()}");
                        if (option.GetOption().ToUpper().Trim().Contains("LIKE"))
                        {
                            builder.Append($"'%{((string)objvalue).Trim()}%'");
                        }
                        else if (option.GetOption().ToUpper().Trim() == "IN")
                        {
                            builder.Append("(");
                            if (objvalue is Array)
                            {
                                string[] array = (string[])objvalue;
                                string _sql_search_count = "";
                                foreach (var item in array)
                                {
                                    _sql_search_count += $"{item},";
                                }
                                _sql_search_count = _sql_search_count.Substring(0, _sql_search_count.Length - 1);
                                builder.Append(_sql_search_count);
                            }
                            else
                            {
                                builder.Append(objvalue.ToString());
                            }
                            builder.Append(")");
                        }
                        else
                        {
                            if (objvalue is int)
                            {
                                builder.Append($"{objvalue.ToString()}");
                            }
                            else
                            {
                                builder.Append($"'{objvalue.ToString()}'");
                            }

                        }
                    }
                }
            }
            return builder.ToString();
        }
        #endregion
    }
}
