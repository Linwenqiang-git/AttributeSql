using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Enums
{
    public enum SymbolEnum
    {
        [Description("(")]
        LeftBrackets = 1,
        [Description(")")]
        RightBrackets = 2
    }
}
