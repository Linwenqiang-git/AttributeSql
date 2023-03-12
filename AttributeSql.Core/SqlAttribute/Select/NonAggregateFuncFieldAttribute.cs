using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Select
{
    /// <summary>
    /// 非聚合函数标记
    /// </summary>
    public class NonAggregateFuncFieldAttribute : FunctionAttribute
    {
        private NonAggregateFunctionEnum _funcName;//函数名称
        private string _tableName;//表名
        private string _fieldName;//字段名
        private string[] _parameters;//非聚合函数的其他参数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="funcName">函数名称</param>
        /// <param name="fieldName">表名</param>
        /// <param name="tableName">字段名</param>
        /// <param name="parameter">非聚合函数的其他参数</param>
        public NonAggregateFuncFieldAttribute(NonAggregateFunctionEnum funcName, string fieldName, string tableName = "", string[] parameters = null)
        {
            _funcName = funcName;
            _tableName = tableName;
            _fieldName = fieldName;
            _parameters = parameters;
        }
        public NonAggregateFuncFieldAttribute(NonAggregateFunctionEnum funcName)
        {
            _funcName = funcName;
        }
        public string GetNonAggregateFuncField()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                switch (_funcName)
                {
                    case NonAggregateFunctionEnum.Group_Concat://分组连接函数
                        if (!string.IsNullOrEmpty(_tableName))
                            sql.Append($"{_funcName.GetDescription()}({_tableName}.{_fieldName})");
                        else
                            sql.Append($"{_funcName}({_fieldName})");
                        break;
                    case NonAggregateFunctionEnum.Date_Foramt://日期格式化函数
                        if (_parameters == null || _parameters.Length == 0)
                            throw new NullReferenceException();
                        if (!string.IsNullOrEmpty(_tableName))
                            sql.Append($"{_funcName.GetDescription()}({_tableName}.{_fieldName},{_parameters[0]})");
                        else
                            sql.Append($"{_funcName.GetDescription()}({_fieldName},{_parameters[0]})");
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            catch (NullReferenceException ex)
            {
                throw new AttrSqlException("函数需要的参数值为空，请检查模型端特性[NonAggregateFuncFieldAttribute]的参数配置！");
            }
            catch (ArgumentException ex)
            {
                throw new AttrSqlException("未定义该函数的操作,请继续完善！");
            }
            return sql.ToString();
        }
    }
}
