using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Select
{
    /// <summary>
    /// 聚合函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class AggregateFuncFieldAttribute : FunctionAttribute
    {
        private string FuncName;//函数名称
        private string TableName;//表名
        private string FieldName;//字段名

        public AggregateFuncFieldAttribute(string _FuncName, string _FieldName, string _TableName = "")
        {
            FuncName = _FuncName;
            TableName = _TableName;
            FieldName = _FieldName;
        }
        public AggregateFuncFieldAttribute(string _FuncName)
        {
            FuncName = _FuncName;
        }
        public string GetAggregateFuncField()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                switch (FuncName.ToUpper())
                {
                    case "SUM":
                    case "COUNT":
                        if (!string.IsNullOrEmpty(TableName))
                            sql.Append($"{FuncName}({TableName}.{FieldName})");
                        else
                            sql.Append($"{FuncName}({FieldName})");
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("需要的条件值为空，请检查模型端特性[AggregateFuncFieldAttribute]的参数配置！");
            }
            catch (ArgumentException ex)
            {
                throw new Exception("未定义该函数的操作,请继续完善！");
            }
            return sql.ToString();
        }
    }
}
