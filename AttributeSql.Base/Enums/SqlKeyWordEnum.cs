using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Enums
{
    /// <summary>
    /// sql 关键字
    /// </summary>
    public enum SqlKeyWordEnum
    {
        [Description("Select")]
        Select = 0,
        [Description("As")]
        As,
        [Description("From")]
        From,
        [Description("Join")]
        Join,
        [Description("Left Join")]
        Left_Join,
        [Description("Right Join")]
        Right_Join,
        [Description("Inner Join")]
        Inner_Join,
        [Description("On")]
        On,
        [Description("Where")]
        Where,
        [Description("Group By")]
        Group_By,        
        [Description("Having")]
        Having,
        [Description("Delete")]
        Delete,
        [Description("Update")]
        Update,
        [Description("Set")]
        Set,
        [Description("Insert Into")]
        Insert_Into,
        [Description("Values")]
        Values,
        [Description("Is Null")]
        Is_Null,
        [Description("Is Not Null")]
        Is_Not_Null,
        [Description("Null")]
        Null,
        [Description("Order By")]
        Order_By,
        [Description("Asc")]
        Asc,
        [Description("Desc")]
        Desc
    }
}
