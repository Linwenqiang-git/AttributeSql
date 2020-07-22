using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.Mysql.Repository
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
                int length = parameters.GetType().GetProperties().Length;
                MySqlParameter[] param = new MySqlParameter[length];
                int cursor = 0;
                foreach (var item in parameters.GetType().GetProperties())
                {
                    param[cursor] = new MySqlParameter();
                    param[cursor].ParameterName = $"@{item.Name}";
                    param[cursor].Value = item.GetValue(parameters, null);
                    //参数类型是list的，需要转换成string
                    if (param[cursor].Value?.GetType() == typeof(List<int>))
                        param[cursor].Value = ((List<int>)param[cursor].Value).ToContainString();
                    else if (param[cursor].Value?.GetType() == typeof(List<byte>))
                        param[cursor].Value = ((List<byte>)param[cursor].Value).ToContainString();
                    else if (param[cursor].Value?.GetType() == typeof(List<long>))
                        param[cursor].Value = ((List<long>)param[cursor].Value).ToContainString();
                    else if (param[cursor].Value?.GetType() == typeof(List<string>))
                        param[cursor].Value = ((List<string>)param[cursor].Value).ToContainString();
                    ++cursor;
                }
                command.Parameters.AddRange(param);
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
