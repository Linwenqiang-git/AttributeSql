using AttributeSql.Base.Exceptions;
using AttributeSql.Base.Extensions;
using AttributeSql.Core.Enums;

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
        private AggregateFunctionEnum _funcName;//函数名称
        private string _tableName;//表名
        private string _fieldName;//字段名

        public AggregateFuncFieldAttribute(AggregateFunctionEnum funcName, string fieldName, string tableName = "")
        { 
            _funcName = funcName;
            _tableName = tableName;
            _fieldName = fieldName;
        }
        public AggregateFuncFieldAttribute(AggregateFunctionEnum funcName)
        {
            _funcName = funcName;
        }
        public string GetAggregateFuncField()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                switch (_funcName)
                {
                    case AggregateFunctionEnum.Max:
                    case AggregateFunctionEnum.Min:
                    case AggregateFunctionEnum.Sum:
                    case AggregateFunctionEnum.Count:
                        if (!string.IsNullOrEmpty(_tableName))
                            sql.Append($"{_funcName.GetDescription()}({_tableName}.{_fieldName})");
                        else
                            sql.Append($"{_funcName}({_fieldName})");
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            catch (NullReferenceException ex)
            {
                throw new AttrSqlException("需要的条件值为空，请检查模型端特性[AggregateFuncFieldAttribute]的参数配置！");
            }
            catch (ArgumentException ex)
            {
                throw new AttrSqlException("未定义该函数的操作,请继续完善！");
            }
            return sql.ToString();
        }
    }
}
