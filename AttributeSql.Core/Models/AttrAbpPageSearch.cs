using AttributeSql.Base.Enums;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Models
{
    public class AttrAbpPageSearch
    {
        /// <summary>
        /// 仅显示正常数据
        /// </summary>
        [DbFieldName("a","isdeleted")]
        [OperationCode(OperatorEnum.Equal)]
        public bool? Isdeleted { get; set; } = false;
    }
}
