using System.Collections.Generic;
using System.Text;

using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.Validator;

namespace AttributeSql.Core.SqlAttributeExtensions.QueryExtensions
{
    internal static class FiledNotAllowRepeatExtension
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
                        builder.Append($"{SqlKeyWordEnum.Select.GetDescription()} {table.GetPrimaryKey()} {SqlKeyWordEnum.From.GetDescription()} {table.GetTableName()} ");
                        builder.Append($"{SqlKeyWordEnum.Where.GetDescription()} ({table.GetDbFieldName()}=@{prop.Name} ");
                    }
                    else
                    {
                        builder.Append($"{SqlKeyWordEnum.Select.GetDescription()} {table.GetPrimaryKey()} {SqlKeyWordEnum.From.GetDescription()} {table.GetTableName()} ");
                        builder.Append($"{SqlKeyWordEnum.Where.GetDescription()} {SymbolEnum.LeftBrackets.GetDescription()}");

                        for (int i = 0; i < FieldNames.Length; i++)
                        {
                            if (i + 1 != FieldNames.Length)
                                builder.Append($"{FieldNames[i]} = @{FieldNames[i]} {RelationEume.And.GetDescription()} ");
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
                    if (builder.ToString().Contains(SqlKeyWordEnum.Where.GetDescription()))
                        builder.Append($" {SymbolEnum.RightBrackets.GetDescription()}");
                    if (!string.IsNullOrEmpty(SoftDeleteField))
                        builder.Append($" {RelationEume.And.GetDescription()} {SoftDeleteField} = {SoftDeleteFieldValue}");
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
        internal static string NotAllowKeySql(this AttrBaseModel model, int checkSqltag = 1)
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
                            builder.Append($"{SqlKeyWordEnum.Select.GetDescription()} {table.GetPrimaryKey()} {SqlKeyWordEnum.From.GetDescription()} {table.GetTableName()} ");
                            builder.Append($"{SqlKeyWordEnum.Where.GetDescription()} {SymbolEnum.LeftBrackets.GetDescription()}{table.GetPrimaryKey()}=@{table.GetPrimaryKey()} ");
                            builder.Append($"{RelationEume.And.GetDescription()} {table.GetDbFieldName()}=@{prop.Name} ");
                        }
                        else
                        {
                            builder.Append($"{SqlKeyWordEnum.Select.GetDescription()} {table.GetPrimaryKey()} {SqlKeyWordEnum.From.GetDescription()} {table.GetTableName()} ");
                            builder.Append($"{SqlKeyWordEnum.Where.GetDescription()} ({table.GetPrimaryKey()}=@{table.GetPrimaryKey()} {RelationEume.And.GetDescription()} ");

                            for (int i = 0; i < FieldNames.Length; i++)
                            {
                                if (i + 1 != FieldNames.Length)
                                    builder.Append($"{FieldNames[i]} = @{FieldNames[i]} {RelationEume.And.GetDescription()} ");
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
                        if (builder.ToString().Contains(SqlKeyWordEnum.Where.GetDescription()))
                            builder.Append($" {SymbolEnum.RightBrackets.GetDescription()}");
                        if (!string.IsNullOrEmpty(SoftDeleteField))
                            builder.Append($" {RelationEume.And.GetDescription()} {SoftDeleteField} {OperatorEnum.Equal.GetDescription()} {SoftDeleteFieldValue}");
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
        internal static string GetErrorMsg(this AttrBaseModel model, int num = 1)
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
