using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlAttribute.Select
{
    /// <summary>
    /// 运算操作（一个特性只允许绑定一次）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class OperationAttribute : Attribute
    {
        private string Operator;//运算符
        private string LeftField;//左侧运算字段
        private string RightField;//右侧运算字段
        private int Decimal;//小数点位数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_LeftField">左侧运算字段(需自行加表别名)</param>
        /// <param name="_Operator"></param>
        /// <param name="_RightField">右侧运算字段(需自行加表别名)</param>
        /// <param name="_Decimal">小数点位数（默认为0则不保留）</param>
        public OperationAttribute(string _LeftField, string _Operator, string _RightField, int _Decimal = 0)
        {
            Operator = _Operator;
            LeftField = _LeftField;
            RightField = _RightField;
            Decimal = _Decimal;
        }
        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <returns></returns>
        public string GetExpression()
        {
            if (string.IsNullOrEmpty(Operator))
                throw new Exception($"未设置运算操作符,请检查model特性配置!");
            if (Operator.Trim() != "+" && Operator.Trim() != "-" && Operator.Trim() != "*" && Operator.Trim() != "/")
            {
                throw new Exception($"无法识别的运算操作符：[{Operator}],请检查model特性配置!");
            }
            string Expression = string.Empty;
            if (Operator.Trim() != "/")
            {
                Expression = $" {LeftField} {Operator} {RightField} ";
            }
            else
            {
                //除法运算防止分母为0
                StringBuilder sql = new StringBuilder();
                sql.Append($"CASE WHEN {RightField} IS NULL OR {RightField} = 0 THEN {LeftField} ELSE ");
                sql.Append($"{LeftField} {Operator} {RightField} END ");
                Expression = sql.ToString();
            }
            if (Decimal != 0)
            {
                return $"Round({Expression},{Decimal})";
            }
            return Expression;
        }

    }
}
