using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.SqlAttribute.JoinTable;
using AttributeSqlDLL.SqlAttribute.OrderBy;
using AttributeSqlDLL.SqlAttribute.Select;
using AttributeSqlDLL.SqlAttribute.Where;

namespace AttributeSql.Model
{
    [DefaultOrderBy("C02_CustomerId", "desc", "p")]
    public class OrderPageSearch : AttrPageSearch
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        [OperationCode("in")]
        [TableByName("p")]
        [DbFieldName("dbfieldname_CustomerId")]
        public List<long> Customer { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>        
        [OperationCode("=")]
        [TableByName("p")]
        [DbFieldName("dbfieldname_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// 项目流程ID
        /// </summary>
        [OperationCode("=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// 支付日期-开始
        /// </summary>
        [OperationCode(">=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeStart { get; set; }
        /// <summary>
        /// 支付日期-结束
        /// </summary>
        [OperationCode("<=")]
        [TableByName("o")]
        [DbFieldName("dbfieldname_FinishTime")]
        public DateTime? PayTimeEnd { get; set; }
        /// <summary>
        /// 支付状态 1待支付2支付完成3支付取消4待退款5已退款6退款取消
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
        /// 客户ID
        /// </summary>
        [TableByName("p")]
        [DbFieldName("C02_CustomerId")]
        public long? CustomerId { get; set; }
        /// <summary> 
        /// 订单编号
        /// </summary>
        [DbFieldName("(select R03_PayRecordNo from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public string OrderPayNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [DbFieldName("(select R03_PayMode from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public int? PayMode { get; set; }
        /// <summary>
        /// 支付账号
        /// </summary>
        [DbFieldName("(select R03_PayAccount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public string PayAccount { get; set; }
        /// <summary>
        /// 订单标题
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Title")]
        public string Title { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Body")]
        public string Body { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_Amount")]
        public double Amount { get; set; }
        /// <summary>
        /// 订单实收金额
        /// </summary>        
        [DbFieldName("(select R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public double? PayAmount { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>
        [TableByName("p")]
        [DbFieldName("P01_ProductId")]
        public long? ProductId { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        [TableByName("o")]
        [DbFieldName("P02_ProductFlowId")]
        public long? ProductFlowId { get; set; }
        /// <summary>
        /// 订单最终的状态
        /// 1待支付2支付完成3支付取消4待退款5已退款6退款取消
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_PayStatus")]
        public long? PayStatus { get; set; }
        /// <summary>
        /// 支付记录支付情况
        /// </summary>
        [DbFieldName("(select R03_PayStatus from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
        public int R03_PayStatus { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [TableByName("o")]
        public long? R02_OrderPayId { get; set; }
        /// <summary>
        /// 订单完成/取消时间
        /// </summary>
        [TableByName("o")]
        [DbFieldName("R02_FinishTime")]
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 收款方账户
        /// </summary>
        [TableByName("p")]
        [DbFieldName("R01_Account")]
        public string Account { get; set; }
    }
}
