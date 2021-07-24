using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Text;

namespace AttributeSqlDLL.Oracle.Repository
{
    /// <summary>
    /// 创建数据库连接
    /// </summary>
    public static class CreateConn
    {
        public static IDbCommand CreateCommand(this IDbConnection conn, string sql)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }
        public static IDbCommand CreateCommand<TParamter>(this IDbConnection conn, string sql, TParamter parameters = null)
            where TParamter : class
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            CombineParams(ref cmd, parameters);
            return cmd;

        }
        public static void CombineParams<TParamter>(ref IDbCommand command, TParamter parameters) where TParamter : class
        {
            if (parameters != null)
            {               
                foreach (var item in parameters.GetType().GetProperties())
                {
                    var oracleParameter = new OracleParameter();
                    oracleParameter.ParameterName = $"@{item.Name}";
                    oracleParameter.Value = item.GetValue(parameters, null);
                    //参数类型是list的，需要转换成string
                    if (oracleParameter.Value?.GetType() == typeof(List<int>))
                        oracleParameter.Value = ((List<int>)oracleParameter.Value).ToContainString();
                    else if (oracleParameter.Value?.GetType() == typeof(List<byte>))
                        oracleParameter.Value = ((List<byte>)oracleParameter.Value).ToContainString();
                    else if (oracleParameter.Value?.GetType() == typeof(List<long>))
                        oracleParameter.Value = ((List<long>)oracleParameter.Value).ToContainString();
                    else if (oracleParameter.Value?.GetType() == typeof(List<string>))
                        oracleParameter.Value = ((List<string>)oracleParameter.Value).ToContainString();
                    command.Parameters.Add(oracleParameter);
                }                
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
