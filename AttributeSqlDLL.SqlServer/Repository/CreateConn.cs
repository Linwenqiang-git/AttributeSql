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
                    var mySqlParameter = new SqlParameter();
                    mySqlParameter.ParameterName = $"@{item.Name}";
                    mySqlParameter.Value = item.GetValue(parameters, null);
                    //参数类型是list的，需要转换成string
                    if (mySqlParameter.Value?.GetType() == typeof(List<int>))
                        mySqlParameter.Value = ((List<int>)mySqlParameter.Value).ToContainString();
                    else if (mySqlParameter.Value?.GetType() == typeof(List<byte>))
                        mySqlParameter.Value = ((List<byte>)mySqlParameter.Value).ToContainString();
                    else if (mySqlParameter.Value?.GetType() == typeof(List<long>))
                        mySqlParameter.Value = ((List<long>)mySqlParameter.Value).ToContainString();
                    else if (mySqlParameter.Value?.GetType() == typeof(List<string>))
                        mySqlParameter.Value = ((List<string>)mySqlParameter.Value).ToContainString();
                    command.Parameters.Add(mySqlParameter);
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
