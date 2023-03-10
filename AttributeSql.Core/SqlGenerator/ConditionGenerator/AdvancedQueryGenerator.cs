using AttributeSql.Base.Enums;
using AttributeSql.Core.Extensions;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Core.SqlAttribute.Where;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using AttributeSql.Base.SqlExecutor;

namespace AttributeSql.Core.SqlGenerator.ConditionGenerator
{
    /// <summary>
    /// 高级查询生成器
    /// </summary>
    internal class AdvancedQueryGenerator<TAdvancedField> : GeneralQueryGenerator
    {
        public IAdvancedQueryBaseField<TAdvancedField>? AdvancedQueryBaseField;
        public AdvancedQueryGenerator(object? obj):base(obj)
        {
            AdvancedQueryBaseField = obj as IAdvancedQueryBaseField<TAdvancedField>;
        }
        /// <summary>
        /// 判断字段是否有值
        /// </summary>
        /// <param name="base._obj"></param>
        /// <param name="ingnorIntDefault"></param>
        /// <returns></returns>
        public override bool FieldHasValue(bool ingnorIntDefault = true)
        {
            if (base._obj == null)
                return false;
            bool hasValue = true;
            //int 和 datetime的默认值单独判断
            if (base._obj is IntField)
            {
                var values = ((IntField)base._obj).Values;
                if (values == null || values.Count == 0)
                    return false;
                if (ingnorIntDefault && !values.Any(s => s != 0))
                    return false;
            }
            else if (base._obj is DateTimeField)
            {
                var values = ((DateTimeField)base._obj).Values;
                if (values == null || values.Count == 0)
                    return false;
                if (!values.Any(s => s != default))
                    return false;
            }
            else
            {                
                if (this.AdvancedQueryBaseField?.Values == null || this.AdvancedQueryBaseField.Values.Count == 0)
                    return false;
            }
            return hasValue;
        }
        #region 构建操作符以及参数化部分
        public override StringBuilder BuildConventionRelationWithRight<TDbContext>([NotNull] PropertyInfo propertyInfo, [NotNull] ISqlExecutor<TDbContext> sqlExecutor, string tableField)
        {
            StringBuilder builder = new StringBuilder(); 

            var advancedQueryField = base._obj as IAdvancedQueryBaseField<TAdvancedField>;
            //List包含多个值，默认使用In
            if (advancedQueryField.Values != null && advancedQueryField.Values.Count > 1)
            {
                builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                builder.Append($" {tableField} IN (@{propertyInfo.Name}) ");
                //builder.Append("FIND_IN_SET");
                //builder.Append($"({tableField},@{propertyInfo.Name})");
            }
            if (advancedQueryField.Values != null && advancedQueryField.Values.Count == 1)
            {
                builder.Append($" {advancedQueryField.Operator.GetDescription()}");// 操作符
                builder.Append($" @{propertyInfo.Name}");//参数化查询
            }
            return builder;
        }
        #endregion
    }
}
