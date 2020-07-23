using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace AttributeSqlDLL.SqlServer.Repository
{
    /// <summary>
    /// 创建数据库连接
    /// </summary>
    public static class CreateConn
    {
        public static DbCommand CreateCommand(this DbConnection conn, string sql)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }
        public static DbCommand CreateCommand<TParamter>(this DbConnection conn, string sql, TParamter parameters = null)
            where TParamter : class
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            CombineParams(ref cmd, parameters);
            return cmd;

        }
        public static void CombineParams<TParamter>(ref DbCommand command, TParamter parameters) where TParamter : class
        {
            if (parameters != null)
            {
                List<SqlParameter> param = new List<SqlParameter>();
                foreach (var item in parameters.GetType().GetProperties())
                {
                    var parameter = new SqlParameter();
                    parameter.ParameterName = $"@{item.Name}";
                    parameter.Value = item.GetValue(parameters, null);
                    //参数类型是list的，需要转换成string
                    if (parameter.Value?.GetType() == typeof(List<int>))
                        parameter.Value = ((List<int>)parameter.Value).ToContainString();
                    else if (parameter.Value?.GetType() == typeof(List<byte>))
                        parameter.Value = ((List<byte>)parameter.Value).ToContainString();
                    else if (parameter.Value?.GetType() == typeof(List<long>))
                        parameter.Value = ((List<long>)parameter.Value).ToContainString();
                    else if (parameter.Value?.GetType() == typeof(List<string>))
                        parameter.Value = ((List<string>)parameter.Value).ToContainString();
                    if (parameter.Value != null)
                        param.Add(parameter);
                }
                command.Parameters.AddRange(param.ToArray());
            }
        }
        public static string ToContainString<T>(this List<T> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.Append($"{item},");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
    }
}
