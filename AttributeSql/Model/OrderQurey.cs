using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Core.Model;
using AttributeSqlDLL.Core.SqlAttribute.JoinTable;
using AttributeSqlDLL.Core.SqlAttribute.OrderBy;
using AttributeSqlDLL.Core.SqlAttribute.Select;
using AttributeSqlDLL.Core.SqlAttribute.Where;

namespace AttributeSql.Model
{
    [DefaultOrderBy("C02_CustomerId", "desc", "p")]
    public class OrderPageSearch : AttrPageSearch
    {
        /// <summary>
        /// field1
        /// </summary>
        [OperationCode("in")]
        [TableByName("p")]
        [DbFieldName("dbfieldname_CustomerId")]
        public List<long> Customer { get; set; }
        /// <summary>
        /// field2
        /// </summary>        
        [OperationCode("=")]
        [TableByName("p")]
        [DbFieldName("dbfieldname_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// field3
        /// </summary>
        [OperationCode("=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// field4
        /// </summary>
        [OperationCode(">=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeStart { get; set; }
        /// <summary>
        /// field5
        /// </summary>
        [OperationCode("<=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeEnd { get; set; }
        /// <summary>
        /// field6
        /// </summary>
        [OperationCode("=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_PayStatus")]
        public long? PayStatus { get; set; }        
    }
    [MainTable("R01_Order", "p")]
    [LeftTable("R02_OrderPay", "R01_OrderId", "R01_OrderId", "o")]
    public class OrderSearchResultDto : AttrBaseResult
    {
        /// <summary>
        /// field1
        /// </summary>
        [TableByName("p")]
        [DbFieldName("C02_CustomerId")]
        public long? CustomerId { get; set; }
        /// <summary> 
        /// field2
        /// </summary>
        //[DbFieldName("(select R03_PayRecordNo from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        //public string OrderPayNo { get; set; }
        /// <summary>
        /// field3
        /// </summary>
        //[DbFieldName("(select R03_PayMode from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        //public int? PayMode { get; set; }
        /// <summary>
        /// field4
        /// </summary>
        //[DbFieldName("(select R03_PayAccount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        //public string PayAccount { get; set; }
        /// <summary>
        /// field5
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Title")]
        public string Title { get; set; }
        /// <summary>
        /// field6
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Body")]
        public string Body { get; set; }
        /// <summary>
        /// field7
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Amount")]
        public double Amount { get; set; }
        /// <summary>
        /// field8
        /// </summary>        
        //[DbFieldName("(select top 1 R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC)")]
        //[DbFieldName("(select R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        //public double? PayAmount { get; set; }
        /// <summary>
        /// field9
        /// </summary>
        [TableByName("p")]
        [DbFieldName("P01_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// field10
        /// </summary>
        [TableByName("o")]
        [DbFieldName("P02_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// field11
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_PayStatus")]
        public long? PayStatus { get; set; }
        /// <summary>
        /// field12
        /// </summary>
        //[DbFieldName("(select R03_PayStatus from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        //public int R03_PayStatus { get; set; }
        /// <summary>
        /// field13
        /// </summary>
        [TableByName("o")]
        public long? R02_OrderPayId { get; set; }
        /// <summary>
        /// field14
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_FinishTime")]
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// field15
        /// </summary>
        [TableByName("p")]
        [DbFieldName("R01_Account")]
        public string Account { get; set; }
    }
}
