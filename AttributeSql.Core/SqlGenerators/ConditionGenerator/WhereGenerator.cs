using AttributeSql.Base.Exceptions;
using AttributeSql.Core.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
using AttributeSql.Base.SqlExecutor;
using Volo.Abp.EntityFrameworkCore;
using AttributeSql.Base.Extensions;
using AttributeSql.Base.SpecialSqlGenerators;
using Mapster;
using AttributeSql.Base.Enums;

namespace AttributeSql.Core.SqlGenerators.ConditionGenerator
{
    internal class WhereGenerator
    {
        private AttrPageSearch _searchModel;
        private bool _ingnorIntDefault;
        private readonly string _whereBase;
        public WhereGenerator(AttrPageSearch searchModel, bool ingnorIntDefault = true)
        {
            _searchModel = searchModel;
            _ingnorIntDefault = ingnorIntDefault;
            _whereBase = " WHERE 1=1";
        }
        /// <summary>
        /// 生成Where部分
        /// </summary>
        /// <returns></returns>
        public StringBuilder Generate(ASpecialSqlGenerator specialSqlGenerator) 
        {
            StringBuilder generalQueryBuilder = new StringBuilder();
            generalQueryBuilder.Append(_whereBase);
            var generalQueryProperties = _searchModel.GetType().GetProperties().Where(p => !p.PropertyType.FullName.Contains("AdvancedSearchModels"));
            var advanceQueryProperties = _searchModel.GetType().GetProperties().Where(p =>  p.PropertyType.FullName.Contains("AdvancedSearchModels"));
            foreach (var prop in generalQueryProperties)
            {
                //对没有标记操作特性的字段不操作
                if (prop.GetCustomAttributes(typeof(OperationCodeAttribute), true).Length == 0)
                    continue;
                var obj = prop.GetValue(_searchModel, null);
                if (obj == null)
                    continue;
                GeneralQueryGenerator generalQueryGenerator = new GeneralQueryGenerator(obj);
                //对有值的进行操作
                if (!generalQueryGenerator.FieldHasValue(_ingnorIntDefault))
                    continue;
                generalQueryBuilder.Append($" {RelationEume.And.GetDescription()} ");
                string tableField = generalQueryGenerator.BuildOperatorLeft(prop);
                //generalQueryBuilder.Append(tableField);
                generalQueryBuilder.Append(generalQueryGenerator.BuildConventionRelationWithRight(prop, specialSqlGenerator, tableField));
                //非聚合函数
                if (prop.IsDefined(typeof(NonAggregateFuncAttribute), true))
                {
                    NonAggregateFuncAttribute? aggregateFunc = prop.GetCustomAttributes(typeof(NonAggregateFuncAttribute), true)[0] as NonAggregateFuncAttribute;
                    generalQueryBuilder.Append(aggregateFunc?.GetFuncoperate());
                }
            }
            StringBuilder advanceQueryBuilder = new StringBuilder();
            int advanceQueryPropertyHasValueIndex = 0;
            foreach (var prop in advanceQueryProperties)
            {
                var obj = prop.GetValue(_searchModel, null);
                if (obj == null)
                    continue;
                AdvObject advObj = obj.Adapt<AdvObject>();
                var advancedQueryGenerator = new AdvancedQueryGenerator(advObj);
                if (!advancedQueryGenerator.FieldHasValue(_ingnorIntDefault))
                    continue;
                if (++advanceQueryPropertyHasValueIndex == 1)
                {
                    advanceQueryBuilder.Append($" {advObj?.Relation.GetDescription()} {SymbolEnum.LeftBrackets.GetDescription()}");
                }
                else
                {
                    advanceQueryBuilder.Append($" {advObj?.Relation.GetDescription()}");
                }
                string tableField = advancedQueryGenerator.BuildOperatorLeft(prop);
                advanceQueryBuilder.Append(tableField);
                advanceQueryBuilder.Append(advancedQueryGenerator.BuildConventionRelationWithRight(prop, specialSqlGenerator, tableField));
            }
            if (advanceQueryPropertyHasValueIndex > 0)
                advanceQueryBuilder.Append(SymbolEnum.RightBrackets.GetDescription());
            if (advanceQueryBuilder.Length > 0)
                return generalQueryBuilder.Append(advanceQueryBuilder);
            return generalQueryBuilder;
        }        
    }
}
