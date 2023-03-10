using AttributeSql.Base.Exceptions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Helper
{
    public static class DtToEnumerable
    {
        public static IEnumerable<T> ToEnumerable<T>(DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            string fieldName = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    T t = new T();
                    foreach (PropertyInfo p in propertyInfos)
                    {
                        fieldName = p.Name;
                        if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        {
                            p.SetValue(t, row[p.Name]);
                        }
                    }
                    ts[i] = t;
                    i++;
                }
                catch (Exception ex)
                {
                    throw new AttrSqlException($"查询结果[{fieldName}]类型转换出错,请检查Dto模型参数类型配置：{ex.Message}");
                }
            }
            return ts;
        }
    }
}
