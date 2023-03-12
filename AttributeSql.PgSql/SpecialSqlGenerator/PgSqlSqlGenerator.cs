using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Base.SpecialSqlGenerators;

using Mapster;

using Newtonsoft.Json.Linq;

using Npgsql;

using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace AttributeSql.PgSql.SpecialSqlGenerator
{
    public class PgSqlSqlGenerator : ASpecialSqlGenerator
    {
        #region BuildParameter
        public override DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                return default;
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                var parameterValue = propertyInfo.GetValue(model, null);
                if (parameterValue == null)
                    continue;
                                
                Type fieldType = propertyInfo.PropertyType;
                //常规集合类型
                if (fieldType.Name == "List`1")
                {
                    IEnumerableParameterBuild(parameters, parameterValue as IEnumerable<object>, propertyInfo.Name);
                }
                //高级查询字段
                else if (fieldType.BaseType == typeof(AdvObject) || fieldType == typeof(AdvObject))
                {
                    AdvanceFieldParameterBuild(parameters, parameterValue, propertyInfo.Name);
                }                
                else
                {
                    //常规类型
                    parameters.Add(new NpgsqlParameter($"@{propertyInfo.Name}", parameterValue));
                }
                    
            }
            return parameters.ToArray();
        }
        private void IEnumerableParameterBuild(List<NpgsqlParameter> pgsqlParameters,IEnumerable<object> list,string propertyName)
        {
            int index = 0;
            foreach (var value in list)
            {
                pgsqlParameters.Add(new NpgsqlParameter($"@{propertyName}_{++index}", value));
            }
        }
        private void AdvanceFieldParameterBuild(List<NpgsqlParameter> pgsqlParameters, object obj,string propertyName)
        {
            var advancedQueryBaseField = obj.Adapt<AdvObject>();
            if (advancedQueryBaseField.Values != null)
            {
                if (advancedQueryBaseField.Values.Count() > 1)
                {
                    int index = 1;
                    foreach (var value in advancedQueryBaseField.Values)
                    {
                        pgsqlParameters.Add(new NpgsqlParameter($"@{propertyName}_{index++}", value));
                    }
                }
                else
                {                    
                    pgsqlParameters.Add(new NpgsqlParameter($"@{propertyName}", advancedQueryBaseField.Values.FirstOrDefault()));
                }
            }            
        }
        private string ToContainString<T>(IAdvancedQueryBaseField<T> filed)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in filed.Values)
            {
                builder.Append($"{item},");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
        #endregion

        #region PaginationSql
        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"limit {Size} offset {Offset}";
        }
        #endregion

        #region GeneralQueryRelationBuild
        public override StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($" {tableField}");//字段                
            builder.Append($" {option.GetDescription()}");//操作符                
            switch (option)
            {
                case OperatorEnum.Is:
                    int value = (int)obj;
                    if (value == 1)
                        builder.Append($" NOT NULL ");
                    else if (value == 2)
                        builder.Append($" NULL ");
                    else
                        throw new AttrSqlException("IS 操作符的值只允许为1(NOT NULL)或2(NULL),请检查传入参数的值是否正确！");
                    break;
                case OperatorEnum.Like:
                    if (obj is string)
                        builder.Append($" '%{((string)obj).Trim().Replace("--", "")}%'");
                    else
                        builder.Append($" @{propertyInfo.Name}");//参数化查询
                    break;
                case OperatorEnum.In:
                case OperatorEnum.NotIn:
                    builder.Remove(builder.Length - 2, 2);//去掉操作符                                                                             
                    builder.Append($" {option.GetDescription()} {SymbolEnum.LeftBrackets.GetDescription()}");
                    var valueCount = (obj as IEnumerable<object>).Count();
                    for (int index = 1; index <= valueCount; index++)
                    {
                        if(index < valueCount)
                            builder.Append($"@{propertyInfo.Name}_{index},");
                        else
                            builder.Append($"@{propertyInfo.Name}_{index}");
                    }
                    builder.Append(SymbolEnum.RightBrackets.GetDescription());
                    break;                
                default:
                    builder.Append($" @{propertyInfo.Name}");//参数化查询
                    break;
            }
            return builder;
        }
        #endregion

        #region AdvanceQueryRelationBuild
        public override StringBuilder AdvanceQueryRelationBuild([NotNull] AdvObject advObject, [NotNull] PropertyInfo propertyInfo, string tableField)
        {
            StringBuilder builder = new StringBuilder();
            //Between 类型单独处理
            if (advObject.Operator == OperatorEnum.Between)
            {
                if (advObject.Values.Count() != 2)
                    throw new AttrSqlSyntaxError($"[{advObject.Operator}]操作只能包含两个选项值");
                builder.Append($" {advObject.Operator.GetDescription()}");// 操作符
                builder.Append($" @{propertyInfo.Name}_1 And @{propertyInfo.Name}_2");//参数化查询
                return builder;
            }
            //List包含多个值，默认使用In 和 Not In
            if (advObject.Values.Count() > 1)
            {
                if (advObject.Operator != OperatorEnum.In && advObject.Operator != OperatorEnum.NotIn)
                    throw new AttrSqlSyntaxError($"集合类型不支持[{advObject.Operator}]操作");
                builder.Append($" {advObject.Operator.GetDescription()} ");               
                builder.Append(SymbolEnum.LeftBrackets.GetDescription());
                for (int i = 1; i <= advObject.Values.Count(); i++)
                {
                    if (i < advObject.Values.Count())
                        builder.Append($" @{propertyInfo.Name}_{i},");
                    else
                        builder.Append($" @{propertyInfo.Name}_{i}");
                }
                builder.Append(SymbolEnum.RightBrackets.GetDescription());
                return builder;
            }
            //数量为1的直接构建
            builder.Append($" {advObject.Operator.GetDescription()}");// 操作符
            builder.Append($" @{propertyInfo.Name}");//参数化查询
            return builder;
        }
        #endregion
    }
}
