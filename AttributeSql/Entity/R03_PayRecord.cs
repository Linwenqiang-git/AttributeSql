using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Core.Model;

namespace AttributeSql.Entity
{
    public class R03_PayRecord : AttrEntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long R03_Id { get; set; }
       
        public long R02_OrderPayId { get; set; }
      
        public DateTime? R03_PayFinishTime { get; set; }
   
        public DateTime? R03_ModifyTime { get; set; }

        public string R02_OrderPayNo { get; set; }
     
        public string R03_PayRemark { get; set; }
      
        public long? R03_DeleteId { get; set; }
  
        public byte R03_PayMode { get; set; }
  
        public byte R03_IsValid { get; set; }
     
        public string R03_DeleteBy { get; set; }
       
        public string R03_PayAccount { get; set; }
     
        public long R03_CreateId { get; set; }
  
        public DateTime? R03_DeleteTime { get; set; }

        public string R03_PayBank { get; set; }
  
        public string R03_CreateBy { get; set; }

        public double? R03_PayAmount { get; set; }

        public DateTime R03_CreateTime { get; set; }
   
        public string R03_PayLink { get; set; }
 
        public long? R03_ModifyId { get; set; }

        public string R03_PayRecordNo { get; set; }
    
        public byte R03_PayStatus { get; set; }
   
        public string R03_ModifyBy { get; set; }
    }
}
