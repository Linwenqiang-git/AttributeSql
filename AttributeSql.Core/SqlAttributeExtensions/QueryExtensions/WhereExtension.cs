using AttributeSql.Base.Enums;
using AttributeSql.Base.Extensions;
using AttributeSql.Base.SpecialSqlGenerators;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlGenerators.ConditionGenerator;

using System.Linq.Expressions;

using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Core.SqlAttributeExtensions.QueryExtensions
{
    internal static class WhereExtension
    {
        internal static string whereBase = $" {SqlKeyWordEnum.Where.GetDescription()} 1=1";
        #region 参数化where条件
        /// <summary>
        /// 参数化where条件,只针对标记特性且有值的字段生效
        /// 特性解析规则按照 [聚合函数]([表别名.]{字段名}) {操作符} {参数化变量} [非聚合函数]
        /// 参数化字段的名称需要与模型字段名称一致
        /// </summary>
        /// <param name="model"></param>
        /// <param name="specialSqlGenerator">个性sql生成器</param>
        /// <param name="ingnorIntDefault">int类型的默认值是否忽略,默认忽略</param>
        /// <returns></returns>
        internal static string Where<TResultDto>(this AttrPageSearch searchModel, ASpecialSqlGenerator specialSqlGenerator, bool ingnorIntDefault = true)
            where TResultDto : AttrBaseResult, new()
        {
            WhereGenerator whereGenerator = new WhereGenerator(searchModel, ingnorIntDefault);
            var builder = whereGenerator.Generate(specialSqlGenerator);
            if (builder.ToString() == whereBase)
                return string.Empty;
            AbpDataFilterGenerator abpDataFilterGenerator = new AbpDataFilterGenerator();
            return abpDataFilterGenerator.Generate<TResultDto>(builder.ToString());
        }        
        #endregion
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
            string ExtraWhere = func.Invoke(model) != string.Empty ? $"  {RelationEume.And.GetDescription()} {func.Invoke(model)}" : func.Invoke(model);
            return paraWhere + ExtraWhere;
        }
    }
}
