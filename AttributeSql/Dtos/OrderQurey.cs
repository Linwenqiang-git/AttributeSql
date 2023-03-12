using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AttributeSql.Base.Enums;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.OrderBy;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

namespace AttributeSql.Model
{
    /// <summary>
    /// where部分
    /// </summary>
    [DefaultOrderBy("C02_CustomerId", "desc", "p")]
    public class OrderPageSearch : AttrPageSearch
    {
        /// <summary>
        /// field1
        /// </summary>
        [OperationCode(OperatorEnum.In)]
        [DbFieldName("dbfieldname_CustomerId")]
        public List<long> Customer { get; set; }
        /// <summary>
        /// field2
        /// </summary>        
        [OperationCode(OperatorEnum.Equal)]
        [DbFieldName("dbfieldname_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// field3
        /// </summary>
        [OperationCode(OperatorEnum.Equal)]
        [DbFieldName("dbfieldname_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// field4
        /// </summary>
        [OperationCode(OperatorEnum.GreaterOrEqual)]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeStart { get; set; }
        /// <summary>
        /// field5
        /// </summary>
        [OperationCode(OperatorEnum.LessOrEqual)]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeEnd { get; set; }
        /// <summary>
        /// field6
        /// </summary>
        [OperationCode(OperatorEnum.Equal)]
        [DbFieldName("dbfieldname_PayStatus")]
        public long? PayStatus { get; set; }        
    }
    /// <summary>
    /// select部分
    /// </summary>
    [MainTable("R01_Order", "p")]
    [LeftTable("R02_OrderPay", "R01_OrderId", "R01_OrderId", "o")]
    public class OrderSearchResultDto : AttrBaseResult
    {
        /// <summary>
        /// field1
        /// </summary>
        [DbFieldName("C02_CustomerId")]
        public long? CustomerId { get; set; }
        /// <summary> 
        /// field2
        /// </summary>
        [DbFieldName("(select R03_PayRecordNo from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public string OrderPayNo { get; set; }
        /// <summary>
        /// field3
        /// </summary>
        [DbFieldName("(select R03_PayMode from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public int? PayMode { get; set; }
        /// <summary>
        /// field4
        /// </summary>
        [DbFieldName("(select R03_PayAccount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public string PayAccount { get; set; }
        /// <summary>
        /// field5
        /// </summary>
        [DbFieldName("R02_Title")]
        public string Title { get; set; }
        /// <summary>
        /// field6
        /// </summary>
        [DbFieldName("R02_Body")]
        public string Body { get; set; }
        /// <summary>
        /// field7
        /// </summary>
        [DbFieldName("R02_Amount")]
        public double Amount { get; set; }
        /// <summary>
        /// field8
        /// </summary>        
        //[DbFieldName("(select top 1 R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC)")]
        [DbFieldName("(select R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public double? PayAmount { get; set; }
        /// <summary>
        /// field9
        /// </summary>
        [DbFieldName("P01_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// field10
        /// </summary>
        [DbFieldName("P02_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// field11
        /// </summary>
        [DbFieldName("R02_PayStatus")]
        public long? PayStatus { get; set; }
        /// <summary>
        /// field12
        /// </summary>
        [DbFieldName("(select R03_PayStatus from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public int R03_PayStatus { get; set; }
        /// <summary>
        /// field13
        /// </summary>
        public long? R02_OrderPayId { get; set; }
        /// <summary>
        /// field14
        /// </summary>
        [DbFieldName("R02_FinishTime")]
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// field15
        /// </summary>
        [DbFieldName("R01_Account")]
        public string Account { get; set; }
    }
}
