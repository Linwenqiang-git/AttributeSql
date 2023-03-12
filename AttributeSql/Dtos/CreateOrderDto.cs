using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.CudAttr;

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
    [UpdateTable("tableName")]
    public class OrderUpdate : AttrBaseModel
    {
        /// <summary>
        /// 第一个true表示为更新条件
        /// </summary>
        [DbFiledMapping("DbfieldName1", true)]
        public string R01_OrderNo { get; set; }
        [DbFiledMapping("DbfieldName2")]
        public long? C02_CustomerId { get; set; }
        [DbFiledMapping("DbfieldName3")]
        public long? P01_ProductId { get; set; }
        [DbFiledMapping("DbfieldName4")]
        public long? P02_ProductFlowId { get; set; }
    }

    [InsertTable("tableName")]
    public class InsertDto : AttrBaseModel
    {
        /// <summary>
        /// 第一个true表示为更新条件
        /// </summary>
        [DbFiledMapping("DbfieldName1")]
        public string R01_OrderNo { get; set; }
        [DbFiledMapping("DbfieldName2")]
        public long? C02_CustomerId { get; set; }
        [DbFiledMapping("DbfieldName3")]
        public long? P01_ProductId { get; set; }
        [DbFiledMapping("DbfieldName4")]
        public long? P02_ProductFlowId { get; set; }
    }
}
