using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlAttribute.Select
{
    /// <summary>
    /// 非聚合函数标记
    /// </summary>
    public class NonAggregateFuncFieldAttribute : FunctionAttribute
    {
        private string FuncName;//函数名称
        private string TableName;//表名
        private string FieldName;//字段名
        private string[] Parameter;//非聚合函数的其他参数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_FuncName">函数名称</param>
        /// <param name="_FieldName">表名</param>
        /// <param name="_TableName">字段名</param>
        /// <param name="_Parameter">非聚合函数的其他参数</param>
        public NonAggregateFuncFieldAttribute(string _FuncName, string _FieldName, string _TableName = "", string[] _Parameter = null)
        {
            FuncName = _FuncName;
            TableName = _TableName;
            FieldName = _FieldName;
            Parameter = _Parameter;
        }
        public NonAggregateFuncFieldAttribute(string _FuncName)
        {
            FuncName = _FuncName;
        }
        public string GetNonAggregateFuncField()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                switch (FuncName.ToUpper())
                {
                    case "GROUP_CONCAT"://分组连接函数
                        if (!string.IsNullOrEmpty(TableName))
                            sql.Append($"{FuncName}({TableName}.{FieldName})");
                        else
                            sql.Append($"{FuncName}({FieldName})");
                        break;
                    case "DATE_FORMAT"://日期格式化函数
                        if (Parameter == null || Parameter.Length == 0)
                            throw new NullReferenceException();
                        if (!string.IsNullOrEmpty(TableName))
                            sql.Append($"{FuncName}({TableName}.{FieldName},{Parameter[0]})");
                        else
                            sql.Append($"{FuncName}({FieldName},{Parameter[0]})");
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("函数需要的参数值为空，请检查模型端特性[NonAggregateFuncFieldAttribute]的参数配置！");
            }
            catch (ArgumentException ex)
            {
                throw new Exception("未定义该函数的操作,请继续完善！");
            }
            return sql.ToString();
        }
    }
}
