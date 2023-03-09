using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.Where
{
    /// <summary>
    /// 某字段的操作符，只允许标记一个操作符
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OperationCodeAttribute : Attribute
    {
        private string optioncode = "=";
        public OperationCodeAttribute() { }
        public OperationCodeAttribute(string optioncode)
        {
            this.optioncode = optioncode;
        }
        public string GetOption()
        {
            return optioncode;
        }
    }
    public enum OperateCode
    { 
        
    }
}
