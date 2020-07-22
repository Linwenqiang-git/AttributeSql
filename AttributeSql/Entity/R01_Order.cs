using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Core.Model;

namespace AttributeSql.Entity
{
    public class R01_Order: AttrEntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long R01_OrderId { get; set; }
        public long? C02_CustomerId { get; set; }
        public string R01_IdCard { get; set; }
        public byte R01_IsValid { get; set; }
        public string R01_DeleteBy { get; set; }

        public long P01_ProductId { get; set; }
        public string R01_Identify { get; set; }

        public long R01_CreateId { get; set; }

        public DateTime? R01_DeleteTime { get; set; }

        public string R01_OrderRemark { get; set; }

        public string R01_Education { get; set; }

        public string R01_CreateBy { get; set; }

        public long R01_OrderStatus { get; set; }

        public string R01_School { get; set; }

        public DateTime R01_CreateTime { get; set; }

        public string R01_Account { get; set; }

        public string R01_University { get; set; }

        public long? R01_ModifyId { get; set; }

        public string R01_CustomerName { get; set; }

        public string R01_Major { get; set; }

        public string R01_ModifyBy { get; set; }

        public long? S07_StaffId { get; set; }

        public long? R01_Intention { get; set; }

        public DateTime? R01_ModifyTime { get; set; }

        public string R01_OrderNo { get; set; }

        public string R01_Phone { get; set; }

        public string R01_Origin { get; set; }

        public long? R01_DeleteId { get; set; }
    }
}
