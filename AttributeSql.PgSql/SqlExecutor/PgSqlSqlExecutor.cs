using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttributeSql.Base.SqlExecutor;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Npgsql;
using Volo.Abp.Collections;
using AttributeSql.Base.Models.AdvancedSearchModels;
using NpgsqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;

namespace AttributeSql.PgSql.SqlExecutor
{
    public class PgSqlSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public PgSqlSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        #region Private
        private string ToContainString<T>(List<T> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.Append($"{item},");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
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

        public override DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                return default;
            int length = model.GetType().GetProperties().Length;
            NpgsqlParameter[] param = new NpgsqlParameter[length];
            int cursor = 0;
            foreach (var item in model.GetType().GetProperties())
            {
                param[cursor] = new NpgsqlParameter();
                param[cursor].ParameterName = $"@{item.Name}";
                param[cursor].Value = item.GetValue(model, null);                
                //参数类型是list的，需要转换成string
                Type? fieldType = param[cursor].Value?.GetType();
                if (fieldType == typeof(List<int>))
                    param[cursor].Value = ToContainString((List<int>)param[cursor].Value);
                else if (fieldType == typeof(List<byte>))
                    param[cursor].Value = ToContainString((List<byte>)param[cursor].Value);
                else if (fieldType == typeof(List<long>))
                    param[cursor].Value = ToContainString((List<long>)param[cursor].Value);
                else if (fieldType == typeof(List<string>))
                    param[cursor].Value = ToContainString((List<string>)param[cursor].Value);
                else if (fieldType == typeof(StringField))
                {
                    param[cursor].Value = ToContainString((StringField)param[cursor].Value);
                    param[cursor].NpgsqlDbType = NpgsqlDbType.Varchar;
                }
                else if (fieldType == typeof(GuidField))
                    param[cursor].Value = ToContainString((GuidField)param[cursor].Value);
                else if (fieldType == typeof(IntField))
                    param[cursor].Value = ToContainString((IntField)param[cursor].Value);
                else if (fieldType == typeof(DateTimeField))
                    param[cursor].Value = ToContainString((DateTimeField)param[cursor].Value);
                ++cursor;
            }
            return param;
        }
        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"limit {Size} offset {Offset}";
        }

        public override StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            StringBuilder builder = new StringBuilder();
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
                                                          //+1 加的是空格
                    builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                    //todo: pgsql in 参数化写法
                    builder.Append($" {tableField} IN @{propertyInfo.Name}");                    
                    break;
                case OperatorEnum.NotIn:
                    builder.Remove(builder.Length - 2, 2);//先去掉操作符
                                                          //+1 加的是空格
                    builder.Remove(builder.Length - (tableField.Length + 1), tableField.Length + 1);
                    //todo: pgsql in 参数化写法
                    builder.Append($" {tableField} NOT IN @{propertyInfo.Name}");
                    break;
                default:
                    builder.Append($" @{propertyInfo.Name}");//参数化查询
                    break;
            }
            return builder;
        }

        public override StringBuilder AdvanceQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            throw new NotImplementedException();
        }
    }
}
