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
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AttributeSql.Base.Enums;
using AttributeSql.Base.Exceptions;

namespace AttributeSql.MySql.SqlExecutor
{
    public class MysqlSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public MysqlSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
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
        #endregion

        public override DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                return default(DbParameter[]);
            int length = model.GetType().GetProperties().Length;
            MySqlParameter[] param = new MySqlParameter[length];
            int cursor = 0;
            foreach (var item in model.GetType().GetProperties())
            {
                param[cursor] = new MySqlParameter();
                param[cursor].ParameterName = $"@{item.Name}";
                param[cursor].Value = item.GetValue(model, null);
                //参数类型是list的，需要转换成string
                if (param[cursor].Value?.GetType() == typeof(List<int>))
                    param[cursor].Value = ToContainString((List<int>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<byte>))
                    param[cursor].Value = ToContainString((List<byte>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<long>))
                    param[cursor].Value = ToContainString((List<long>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<string>))
                    param[cursor].Value = ToContainString((List<string>)param[cursor].Value);
                ++cursor;
            }
            return param;
        }

        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"LIMIT {Offset},{Size}";
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

        public override StringBuilder AdvanceQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            throw new NotImplementedException();
        }
    }
}
