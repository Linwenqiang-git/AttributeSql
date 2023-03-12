using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Base.SpecialSqlGenerators;
using AttributeSql.Base.SqlExecutor;

using Mapster;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace AttributeSql.Core.SqlGenerators.ConditionGenerator
{
    /// <summary>
    /// 高级查询生成器
    /// </summary>
    internal class AdvancedQueryGenerator : GeneralQueryGenerator
    {    
        public AdvancedQueryGenerator(object obj) :base(obj){}
        /// <summary>
        /// 判断字段是否有值
        /// </summary>
        /// <param name="base._obj"></param>
        /// <param name="ingnorIntDefault"></param>
        /// <returns></returns>
        public override bool FieldHasValue(bool ingnorIntDefault = true)
        {
            bool hasValue = true;
            //int 和 datetime的默认值单独判断
            if (base._obj is AdvInt || base._obj is AdvDateTime)
            {
                //toto : test
                var values = ((AdvObject)base._obj).Values;
                if (values == null || values.Count() == 0)
                    return false;
                if (ingnorIntDefault && !values.Any(s => s != default))
                    return false;
            }            
            else
            {
                if (((AdvObject)base._obj).Values == null || ((AdvObject)base._obj).Values.Count() == 0)
                    return false;
            }
            return hasValue;
        }

        #region 构建操作符以及参数化部分
        public override StringBuilder BuildConventionRelationWithRight([NotNull] PropertyInfo propertyInfo, [NotNull] ASpecialSqlGenerator specialSqlGenerator, string tableField)
        {                        
            return specialSqlGenerator.AdvanceQueryRelationBuild((AdvObject)base._obj, propertyInfo, tableField);
        }
        #endregion
    }
}
