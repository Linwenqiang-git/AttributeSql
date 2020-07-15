using System.Text;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.GroupHaving;

namespace AttributeSqlDLL.SqlExtendedMethod
{
    internal static class GroupByHavingExtend
    {
        internal static string GroupByHaving(this AttrBaseResult model)
        {
            StringBuilder builder = new StringBuilder();
            //拿到所有标记了该特性的字段
            builder.Append($" Group by ");
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.IsDefined(typeof(GroupByAttribute), true))
                {
                    GroupByAttribute groupBy = prop.GetCustomAttributes(typeof(GroupByAttribute), true)[0] as GroupByAttribute;
                    builder.Append($"{groupBy.GetGroupByField()},");
                }
            }
            if (builder.ToString() == " Group by ")
            {
                builder.Clear();
                return string.Empty;
            }
            else
                builder.Remove(builder.Length - 1, 1);
            //后面继续完善Having部分
            return builder.ToString();
        }
    }
}
