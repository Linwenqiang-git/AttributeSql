using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Model;

namespace AttributeSql.Entity
{
    public class R02_OrderPay: AttrEntityBase
    {
        public long R02_OrderPayId { get; set; }
         
        public long R01_OrderId { get; set; }

        public byte R02_IsValid { get; set; }

        public string R02_DeleteBy { get; set; }

        
        public string R01_OrderNo { get; set; }

        public long R02_CreateId { get; set; }

        public DateTime? R02_DeleteTime { get; set; }

        public long? P02_ProductFlowId { get; set; }

        public string R02_CreateBy { get; set; }

        public string R02_Title { get; set; }

        public DateTime R02_CreateTime { get; set; }

        public string R02_Body { get; set; }
  
        public long? R02_ModifyId { get; set; }
 
        public double R02_Amount { get; set; }

        public string R02_ModifyBy { get; set; }
 
        public long R02_PayStatus { get; set; }
  
        public DateTime? R02_ModifyTime { get; set; }

        public string R02_OrderPayNo { get; set; }

        public DateTime? R02_FinishTime { get; set; }

        public long? R02_DeleteId { get; set; }
    }
}
