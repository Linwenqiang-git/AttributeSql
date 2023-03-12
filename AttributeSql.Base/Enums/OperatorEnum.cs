using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Enums
{
    public enum OperatorEnum
    {
        [Description("=")]
        Equal = 1,
        [Description("!=")]
        NotEqual,
        [Description(">")]
        Greater,
        [Description("<")]
        Less,
        [Description(">=")]
        GreaterOrEqual,
        [Description("<=")]
        LessOrEqual,
        [Description("Like")]
        Like,
        [Description("Is")]
        Is,
        [Description("In")]
        In,
        [Description("Not In")]
        NotIn,
        [Description("BETWEEN")]
        Between
    }
}
