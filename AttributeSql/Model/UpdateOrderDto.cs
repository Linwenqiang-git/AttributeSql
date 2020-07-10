using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.Validator;

namespace AttributeSql.Model
{
    public class UpdateOrderDto : AttrBaseModel
    {
        public long R01_OrderId { get; set; }
        /// <summary>
        /// 添加该特性会自动校验该字段是否有重复
        /// </summary>
        [NotAllowRepeat("R01_OrderNo", "R01_Order", "R01_OrderId","订单编号不允许重复")]
        public string R01_OrderNo { get; set; }
        
        public string R01_OrderRemark { get; set; }
    }
}
