using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Select
{
    /// <summary>
    /// 不需要参数查询的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NonSelectAttribute : Attribute
    {
        public NonSelectAttribute() { }
    }
}
