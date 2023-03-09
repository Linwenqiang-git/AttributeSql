using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Where
{
    /// <summary>
    /// OR操作，该操作的表达式存储在数组中，最后用OR操作符连接
    /// 表达式需要自行编写
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OrOperationAttribute : Attribute
    {
        private string[] expression;
        public OrOperationAttribute(string[] _expression)
        {
            this.expression = _expression;
        }
        public string GetOrOperation()
        {
            if (expression != null && expression.Length > 0)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("(");
                for (int i = 0; i < expression.Length; i++)
                {
                    sql.Append(expression[i]);
                    if (expression.Length != i + 1)
                    {
                        sql.Append($" OR ");
                    }
                }
                sql.Append(")");
                return sql.ToString();
            }
            else
            {
                throw new Exception("未设置OR连接的表达式,请检查Dto模型配置！");
            }
        }
    }
}
