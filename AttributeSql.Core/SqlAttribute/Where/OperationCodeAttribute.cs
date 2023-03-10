using AttributeSql.Base.Enums;
using AttributeSql.Core.Extensions;

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
        private OperatorEnum _operatorEnum;
        public OperationCodeAttribute()
        {
            _operatorEnum = OperatorEnum.Equal;
        }
        public OperationCodeAttribute(OperatorEnum operatorEnum)
        {
            this._operatorEnum = operatorEnum;
        }
        public OperatorEnum GetOperation()
        {
            return _operatorEnum;
        }
        public string GetOperationDescription()
        {
            return _operatorEnum.GetDescription();
        }
    }    
}
