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
using AttributeSql.Base.SpecialSqlGenerators;

namespace AttributeSql.Core.SqlGenerators.ConditionGenerator
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
        public virtual string BuildOperatorLeft([NotNull] PropertyInfo propertyInfo)
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
        private string BuildTableAliasWithField([NotNull] PropertyInfo propertyInfo)
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
        private string BuildOnlyField([NotNull] PropertyInfo propertyInfo)
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
        private string AddAggregateToFields([NotNull] PropertyInfo propertyInfo, string fieldCondition)
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
        public virtual StringBuilder BuildConventionRelationWithRight([NotNull] PropertyInfo propertyInfo, [NotNull] ASpecialSqlGenerator specialSqlGenerator, string tableField)                         
        {
            StringBuilder builder = new StringBuilder();
            OperationCodeAttribute? option = propertyInfo.GetCustomAttributes(typeof(OperationCodeAttribute), true)[0] as OperationCodeAttribute;            
            builder.Append(specialSqlGenerator.GeneralQueryRelationBuild(this._obj, propertyInfo, tableField, option.GetOperation()));
            return builder;            
        }
        #endregion
    }
}
