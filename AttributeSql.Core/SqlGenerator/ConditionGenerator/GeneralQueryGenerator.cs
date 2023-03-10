using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Enums;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AttributeSql.Base.SqlExecutor;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Core.SqlGenerator.ConditionGenerator
{
    /// <summary>
    /// 常规查询生成器
    /// </summary>
    internal class GeneralQueryGenerator
    {
        protected object _obj;
        public GeneralQueryGenerator(object obj)
        {
            _obj = obj;
        }
        /// <summary>
        /// 判断字段是否有值
        /// </summary>
        /// <param name="this._obj"></param>
        /// <param name="ingnorIntDefault"></param>
        /// <returns></returns>
        public virtual bool FieldHasValue(bool ingnorIntDefault = true)
        {
            //判断当前属性是否有值(主要针对string、int和数组类型)
            if (this._obj == null)
                return false;
            bool hasValue = true;
            if (this._obj is string && string.IsNullOrEmpty((string)this._obj))
            {
                hasValue = false;
            }
            else if (this._obj is int && (int)this._obj == 0 && ingnorIntDefault)
            {
                hasValue = false;
            }
            else if (this._obj is DateTime && (DateTime)this._obj == default)
            {
                hasValue = false;
            }
            else if (this._obj is Array)
            {
                Array array = (Array)this._obj;
                if (array == null || array.Length == 0)
                    hasValue = false;
            }
            return hasValue;
        }

        #region 构建操作符左半部分
        /// <summary>
        /// 构建操作符左半部分
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isAppendAnd"></param>
        /// <returns></returns>
        public string BuildOperatorLeft([NotNull] PropertyInfo propertyInfo)
        {
            string fieldCondition = propertyInfo.Name;
            if (propertyInfo.IsDefined(typeof(TableByNameAttribute), true))
            {
                fieldCondition = BuildTableAliasWithField(propertyInfo);
            }
            else if (propertyInfo.IsDefined(typeof(DbFieldNameAttribute), true))
            {
                fieldCondition = BuildOnlyField(propertyInfo);
            }
            return AddAggregateToFields(propertyInfo, fieldCondition);
        }
        /// <summary>
        /// 构建 [表别名.字段] 格式
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isAppendAnd"></param>
        /// <returns></returns>
        protected string BuildTableAliasWithField([NotNull] PropertyInfo propertyInfo)
        {
            TableByNameAttribute? byName = propertyInfo.GetCustomAttributes(typeof(TableByNameAttribute), true)[0] as TableByNameAttribute;
            string fieldCondition = $" {byName?.GetName()}.";
            if (propertyInfo.IsDefined(typeof(DbFieldNameAttribute), true))
            {
                DbFieldNameAttribute? fieldName = propertyInfo.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                fieldCondition += fieldName?.GetDbFieldName();
            }
            else
                fieldCondition += $"{propertyInfo.Name}";
            return fieldCondition;
        }
        /// <summary>
        /// 构建 [字段] 格式
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isAppendAnd"></param>
        /// <returns></returns>
        protected string BuildOnlyField([NotNull] PropertyInfo propertyInfo)
        {
            DbFieldNameAttribute? fieldName = propertyInfo.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
            return fieldName?.GetDbFieldName();
        }
        /// <summary>
        /// 为构建的字段添加聚合方法
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="fieldCondition"></param>
        /// <param name="isAppendAnd"></param>
        /// <returns></returns>
        protected string AddAggregateToFields([NotNull] PropertyInfo propertyInfo, string fieldCondition)
        {
            if (propertyInfo.IsDefined(typeof(AggregateFuncAttribute), true))
            {
                AggregateFuncAttribute? aggregateFunc = propertyInfo.GetCustomAttributes(typeof(AggregateFuncAttribute), true)[0] as AggregateFuncAttribute;
                fieldCondition += $"{aggregateFunc?.GetAggregateFunc()}({fieldCondition})";
            }
            return fieldCondition;
        }
        #endregion

        #region 构建操作符以及参数化部分
        /// <summary>
        /// 构建操作符
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="tableField"></param>
        /// <param name="isAppendAnd"></param>
        /// <exception cref="AttrSqlException"></exception>
        public virtual StringBuilder BuildConventionRelationWithRight<TDbContext>([NotNull] PropertyInfo propertyInfo, [NotNull] ISqlExecutor<TDbContext> sqlExecutor, string tableField) 
                        where TDbContext : IEfCoreDbContext
        {
            StringBuilder builder = new StringBuilder();
            //操作符 [like | in  |  =  | is | find_in_set]
            if (propertyInfo.IsDefined(typeof(OperationCodeAttribute), true))
            {
                //builder.Append($" {tableField}"); //条件字段名
                OperationCodeAttribute? option = propertyInfo.GetCustomAttributes(typeof(OperationCodeAttribute), true)[0] as OperationCodeAttribute;
                builder.Append($" {option?.GetOperationDescription()}");//操作符
                switch (option.GetOperation())
                {
                    case OperatorEnum.Is:
                        int value = (int)this._obj;
                        if (value == 1)
                            builder.Append($" NOT NULL ");
                        else if (value == 2)
                            builder.Append($" NULL ");
                        else
                            throw new AttrSqlException("IS 操作符的值只允许为1(NOT NULL)或2(NULL),请检查传入参数的值是否正确！");
                        break;
                    case OperatorEnum.Like:
                        if (this._obj is string)
                            builder.Append($" '%{((string)this._obj).Trim().Replace("--", "")}%'");
                        else
                            builder.Append($" @{propertyInfo.Name}");//参数化查询
                        break;
                    case OperatorEnum.In:
                        builder.Remove(builder.Length - 2, 2);//先去掉操作符
                                                              //+1 加的是空格
                        builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                        builder.Append("FIND_IN_SET");
                        builder.Append($"({tableField},@{propertyInfo.Name})");
                        break;
                    case OperatorEnum.NotIn:
                        builder.Remove(builder.Length - 2, 2);//先去掉操作符
                                                              //+1 加的是空格
                        builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                        builder.Append("NOT FIND_IN_SET");
                        builder.Append($"({tableField},@{propertyInfo.Name})");
                        break;
                    default:
                        builder.Append($" @{propertyInfo.Name}");//参数化查询
                        break;
                }
            }
            return builder;
            
        }
        #endregion
    }
}
