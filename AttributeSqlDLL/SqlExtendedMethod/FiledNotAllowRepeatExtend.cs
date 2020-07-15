using System.Collections.Generic;
using System.Text;
using AttributeSqlDLL.ExceptionExtension;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.Validator;

namespace AttributeSqlDLL.SqlExtendedMethod
{
    internal static class FiledNotAllowRepeatExtend
    {
        /// <summary>
        /// 校验指定字段的值是否重复
        /// 直接返回查询的sql语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static List<string> NotAllowRepeatSql(this AttrBaseModel model)
        {
            string SoftDeleteField = string.Empty;
            long SoftDeleteFieldValue = 1;
            List<string> checkSqlCollect = new List<string>();
            StringBuilder builder = new StringBuilder();
            foreach (var prop in model.GetType().GetProperties())
            {
                //对没有标记NotAllowRepeatAttribute特性的字段不操作
                if (prop.GetCustomAttributes(typeof(NotAllowRepeatAttribute), true).Length == 0)
                {
                    continue;
                }
                else
                {
                    object[] obj = prop.GetCustomAttributes(typeof(NotAllowRepeatAttribute), true);
                    //设定一个字段只允许标记一个该特性,所以不会出现多个
                    NotAllowRepeatAttribute table = obj[0] as NotAllowRepeatAttribute;
                    if (string.IsNullOrEmpty(table.GetPrimaryKey()))
                    {
                        throw new AttrSqlException("未设置NotAllowRepeatAttribute特性的表主键字段，请检查特性标记！");
                    }
                    if (string.IsNullOrEmpty(table.GetTableName()))
                    {
                        throw new AttrSqlException("未设置NotAllowRepeatAttribute特性的表名称，请检查特性标记！");
                    }
                   
                    var FieldNames = table.GetDbFieldNames();
                    if (FieldNames == null || FieldNames.Length == 0)
                    {
                        builder.Append($"SELECT {table.GetPrimaryKey()} FROM {table.GetTableName()} ");
                        builder.Append($"WHERE ({table.GetDbFieldName()}=@{prop.Name} ");                            
                    }
                    else
                    {
                        builder.Append($"SELECT {table.GetPrimaryKey()} FROM {table.GetTableName()} ");
                        builder.Append($"WHERE (");

                        for (int i = 0; i < FieldNames.Length; i++)
                        {
                            if (i + 1 != FieldNames.Length)
                                builder.Append($"{FieldNames[i]} = @{FieldNames[i]} AND ");
                            else
                                builder.Append($"{FieldNames[i]} = @{FieldNames[i]} ");
                        }
                    }
                    //添加软删除字段的
                    if (string.IsNullOrEmpty(SoftDeleteField) && !string.IsNullOrEmpty(table.GetSoftDeleteFieldName()))
                    {
                        SoftDeleteField = table.GetSoftDeleteFieldName();
                        SoftDeleteFieldValue = table.GetSoftDeleteFieldValue();
                    }
                    //补上where条件的括号以及软删除字段  
                    if (builder.ToString().Contains("WHERE"))
                        builder.Append($" )");
                    if (!string.IsNullOrEmpty(SoftDeleteField))
                        builder.Append($" AND {SoftDeleteField} = {SoftDeleteFieldValue}");
                    checkSqlCollect.Add(builder.ToString());
                    SoftDeleteField = string.Empty;                    
                    builder.Clear();
                }
            }
            return checkSqlCollect;
        }
        /// <summary>
        /// 根据主键查询该名称是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <param name="checkSqltag"></param>
        /// <returns></returns>
        internal static string NotAllowKeySql(this AttrBaseModel model,int checkSqltag = 1)
        {
            string SoftDeleteField = string.Empty;
            long SoftDeleteFieldValue = 1;
            int attrNum = 0;
            StringBuilder builder = new StringBuilder();
            foreach (var prop in model.GetType().GetProperties())
            {
                //对没有标记NotAllowRepeatAttribute特性的字段不操作
                if (prop.GetCustomAttributes(typeof(NotAllowRepeatAttribute), true).Length == 0)
                {
                    continue;
                }
                else
                {                    
                    //找到要检查的特性
                    if (++attrNum == checkSqltag)
                    {
                        object[] obj = prop.GetCustomAttributes(typeof(NotAllowRepeatAttribute), true);
                        NotAllowRepeatAttribute table = obj[0] as NotAllowRepeatAttribute;
                        if (string.IsNullOrEmpty(table.GetPrimaryKey()))
                        {
                            throw new AttrSqlException("未设置NotAllowRepeatAttribute特性的表主键字段，请检查特性标记！");
                        }
                        if (string.IsNullOrEmpty(table.GetTableName()))
                        {
                            throw new AttrSqlException("未设置NotAllowRepeatAttribute特性的表名称，请检查特性标记！");
                        }

                        var FieldNames = table.GetDbFieldNames();
                        if (FieldNames == null || FieldNames.Length == 0)
                        {
                            builder.Append($"SELECT {table.GetPrimaryKey()} FROM {table.GetTableName()} ");
                            builder.Append($"WHERE ({table.GetPrimaryKey()}=@{table.GetPrimaryKey()} ");
                            builder.Append($"AND {table.GetDbFieldName()}=@{prop.Name} ");
                        }
                        else
                        {
                            builder.Append($"SELECT {table.GetPrimaryKey()} FROM {table.GetTableName()} ");
                            builder.Append($"WHERE ({table.GetPrimaryKey()}=@{table.GetPrimaryKey()} AND ");

                            for (int i = 0; i < FieldNames.Length; i++)
                            {
                                if (i + 1 != FieldNames.Length)
                                    builder.Append($"{FieldNames[i]} = @{FieldNames[i]} AND ");
                                else
                                    builder.Append($"{FieldNames[i]} = @{FieldNames[i]} ");
                            }
                        }

                        //添加软删除字段的
                        if (string.IsNullOrEmpty(SoftDeleteField) && !string.IsNullOrEmpty(table.GetSoftDeleteFieldName()))
                        {
                            SoftDeleteField = table.GetSoftDeleteFieldName();
                            SoftDeleteFieldValue = table.GetSoftDeleteFieldValue();
                        }
                        //补上where条件的括号以及软删除字段
                        if (builder.ToString().Contains("WHERE"))
                            builder.Append($" )");
                        if (!string.IsNullOrEmpty(SoftDeleteField))
                            builder.Append($" AND {SoftDeleteField} = {SoftDeleteFieldValue}");
                        break;
                    }
                    
                }
            }
            return builder.ToString();
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="num">表示第几个不允许为空的属性</param>
        /// <returns></returns>
        internal static string GetErrorMsg(this AttrBaseModel model,int num = 1)
        {
            int flag = 1;
            StringBuilder builder = new StringBuilder();
            foreach (var prop in model.GetType().GetProperties())
            {
                if (!prop.IsDefined(typeof(NotAllowRepeatAttribute), true))
                {
                    continue;
                }
                if (flag == num)
                {
                    NotAllowRepeatAttribute noRepeat = prop.GetCustomAttributes(typeof(NotAllowRepeatAttribute), true)[0] as NotAllowRepeatAttribute;
                    builder.Append($"{noRepeat.GetMsg()}");
                    break;
                }
                ++flag;
            }
            return builder.ToString();
        }
    }
}
