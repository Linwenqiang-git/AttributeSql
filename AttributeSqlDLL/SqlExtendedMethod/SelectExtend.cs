using System.Text;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.JoinTable;
using AttributeSqlDLL.SqlAttribute.Select;

namespace AttributeSqlDLL.SqlExtendedMethod
{
    internal static class SelectExtend
    {
        /// <summary>
        /// 获取查询字段
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string Select(this AttrBaseResult model)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            foreach (var prop in model.GetType().GetProperties())
            {
                //若该属性没有标记字段，则直接使用该字段
                if (prop.GetCustomAttributes(true).Length == 0)
                {
                    builder.Append($"{prop.Name},");
                    continue;
                }
                //是否使用函数
                if (prop.IsDefined(typeof(NonAggregateFuncFieldAttribute), true))
                {
                    NonAggregateFuncFieldAttribute funcFieldAttribute = prop.GetCustomAttributes(typeof(NonAggregateFuncFieldAttribute), true)[0] as NonAggregateFuncFieldAttribute;
                    builder.Append($"{funcFieldAttribute.GetNonAggregateFuncField()} AS ");
                    builder.Append($"{prop.Name},");
                    continue;
                }
                //是否使用聚合函数
                if (prop.IsDefined(typeof(AggregateFuncFieldAttribute), true))
                {
                    AggregateFuncFieldAttribute funcFieldAttribute = prop.GetCustomAttributes(typeof(AggregateFuncFieldAttribute), true)[0] as AggregateFuncFieldAttribute;
                    builder.Append($"{funcFieldAttribute.GetAggregateFuncField()} AS ");
                    builder.Append($"{prop.Name},");
                    continue;
                }
                //是否使用运算符操作
                if (prop.IsDefined(typeof(OperationAttribute), true))
                {
                    OperationAttribute operation = prop.GetCustomAttributes(typeof(OperationAttribute), true)[0] as OperationAttribute;
                    builder.Append($"{operation.GetExpression()} AS {prop.Name},");
                    continue;
                }
                //表别名.字段名 AS 字段别名
                if (prop.IsDefined(typeof(TableByNameAttribute), true))
                {
                    TableByNameAttribute byName = prop.GetCustomAttributes(typeof(TableByNameAttribute), true)[0] as TableByNameAttribute;
                    builder.Append($"{byName.GetName()}.");
                }
                if (prop.IsDefined(typeof(DbFieldNameAttribute), true))
                {
                    DbFieldNameAttribute fieldName = prop.GetCustomAttributes(typeof(DbFieldNameAttribute), true)[0] as DbFieldNameAttribute;
                    builder.Append($"{fieldName.GetDbFieldName()} AS ");
                    builder.Append($"{prop.Name},");
                }
                else
                {
                    builder.Append($"{prop.Name},");
                }

            }
            //移除最后一个逗号
            builder.Remove(builder.Length - 1, 1);
            builder.Append(" ");
            return builder.ToString();
        }
    }
}
