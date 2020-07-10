using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Model;

namespace AttributeSql.Model
{
    public class CreateOrderDto : AttrBaseModel
    {
        public double Amount { get; set; }
        public string R01_OrderNo { get; set; }
        public long? C02_CustomerId { get; set; }
        public long? P01_ProductId { get; set; }
        public long? P02_ProductFlowId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string R01_OrderRemark { get; set; }
        public long? R01_OrderStatus { get; set; } = 1;
        public string R01_Account { get; set; }
        public string R01_CustomerName { get; set; }
        public long? S07_StaffId { get; set; }
        public string R01_Phone { get; set; }
        public string R01_IdCard { get; set; }
        public string R01_Identify { get; set; }
        public string R01_Education { get; set; }
        public string R01_School { get; set; }
        public string R01_University { get; set; }
        public string R01_Major { get; set; }
        public long? R01_Intention { get; set; }
        public string R01_Origin { get; set; }
        public long? R01_IsValid { get; set; } = 1;
        public long? R01_CreateId { get; set; }
        public string R01_CreateBy { get; set; }
    }
}
