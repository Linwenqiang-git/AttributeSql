using AttributeSql.Base.Exceptions;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Select
{
    /// <summary>
    /// 运算操作（一个特性只允许绑定一次）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class OperationAttribute : Attribute
    {
        private string _operator;//运算符
        private string _leftField;//左侧运算字段
        private string _rightField;//右侧运算字段
        private int _decimalPlaces;//小数点位数
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_LeftField">左侧运算字段(需自行加表别名)</param>
        /// <param name="_Operator"></param>
        /// <param name="_RightField">右侧运算字段(需自行加表别名)</param>
        /// <param name="_Decimal">小数点位数（默认为0则不保留）</param>
        public OperationAttribute(string leftField, string operate, string rightField, int decimalPlaces = 0)
        {
            _leftField = leftField;
            _operator = operate;
            _rightField = rightField;
            _decimalPlaces = decimalPlaces;
        }
        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <returns></returns>
        public string GetExpression()
        {
            if (string.IsNullOrEmpty(_operator))
                throw new AttrSqlException($"未设置运算操作符,请检查model特性配置!");
            if (_operator.Trim() != "+" && _operator.Trim() != "-" && _operator.Trim() != "*" && _operator.Trim() != "/")
            {
                throw new AttrSqlException($"无法识别的运算操作符：[{_operator}],请检查model特性配置!");
            }
            string Expression = string.Empty;
            if (_operator.Trim() != "/")
            {
                Expression = $" {_leftField} {_operator} {_rightField} ";
            }
            else
            {
                //除法运算防止分母为0
                StringBuilder sql = new StringBuilder();
                sql.Append($"CASE WHEN {_rightField} IS NULL OR {_rightField} = 0 THEN {_leftField} ELSE ");
                sql.Append($"{_leftField} {_operator} {_rightField} END ");
                Expression = sql.ToString();
            }
            if (_decimalPlaces > 0)
            {
                return $"Round({Expression},{_decimalPlaces})";
            }
            return Expression;
        }

    }
}
