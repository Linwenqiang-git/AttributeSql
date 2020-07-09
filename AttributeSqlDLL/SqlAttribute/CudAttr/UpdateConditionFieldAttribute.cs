using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlAttribute.CudAttr
{
    /// <summary>
    /// 指定字段为更新条件
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class UpdateConditionFieldAttribute : Attribute
    {
        public UpdateConditionFieldAttribute()
        { }
    }
}
