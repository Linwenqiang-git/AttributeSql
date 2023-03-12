using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Base.SpecialSqlGenerators;

using Mapster;

using MySqlConnector;

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace AttributeSql.MySql.SpecialSqlGenerator
{
    public class MySqlSqlGenerator : ASpecialSqlGenerator
    {
        #region BuildParameter
        public override DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                return default;        
            List<MySqlParameter> parameters = new List<MySqlParameter>();
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
                    parameters.Add(new MySqlParameter($"@{propertyInfo.Name}", parameterValue));
                }
            }
            return parameters.ToArray();
        }
        private void IEnumerableParameterBuild(List<MySqlParameter> pgsqlParameters, IEnumerable<object> list, string propertyName)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.Append($"{item},");
            }
            builder.Remove(builder.Length - 1, 1);
            pgsqlParameters.Add(new MySqlParameter($"@{propertyName}", builder.ToString()));
        }
        private void AdvanceFieldParameterBuild(List<MySqlParameter> pgsqlParameters, object obj, string propertyName)
        {
            var advancedQueryBaseField = obj.Adapt<AdvObject>();
            if (advancedQueryBaseField.Values != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in advancedQueryBaseField.Values)
                {
                    builder.Append($"{item},");
                }
                builder.Remove(builder.Length - 1, 1);
                pgsqlParameters.Add(new MySqlParameter($"@{propertyName}", builder.ToString()));
            }
        }
        #endregion

        #region PaginationSql
        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"LIMIT {Offset},{Size}";
        }
        #endregion

        #region GeneralQueryRelationBuild
        public override StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            //操作符 [like | in  |  =  | is | find_in_set]
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
                    builder.Remove(builder.Length - 2, 2);//先去掉操作符                                                          
                    builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);//+1 加的是空格
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
            return builder;
        }
        #endregion

        #region AdvanceQueryRelationBuild
        public override StringBuilder AdvanceQueryRelationBuild([NotNull] AdvObject advObject, [NotNull] PropertyInfo propertyInfo, string tableField)
        {
            StringBuilder builder = new StringBuilder();
            if (advObject.Operator == OperatorEnum.Between)
            {
                throw new AttrSqlException($"暂不支持[{advObject.Operator}]操作");
            }
            //List包含多个值，默认使用In
            if (advObject.Values.Count() > 1)
            {
                builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                if (advObject.Operator == OperatorEnum.In)
                    builder.Append("FIND_IN_SET");
                else if (advObject.Operator == OperatorEnum.NotIn)
                    builder.Append("NOT FIND_IN_SET");
                else
                    throw new AttrSqlSyntaxError($"集合类型不支持[{advObject.Operator}]操作");
                builder.Append($"({tableField},@{propertyInfo.Name})");
                return builder;
            }
            builder.Append($" {advObject.Operator.GetDescription()}");// 操作符
            builder.Append($" @{propertyInfo.Name}");//参数化查询            
            return builder;
        }
        #endregion
    }
}
