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
        [DbFieldName("isdeleted")]
        [OperationCode(OperatorEnum.Equal)]
        public bool? Isdeleted { get; set; }
        /// <summary>
        /// 租户Id
        /// </summary>
        [DbFieldName("tenantid")]
        [OperationCode(OperatorEnum.Equal)]
        public Guid? Tenantid { get; set; }
    }
}
