# AttributeSql 特性sql
## Summary
c#开发，基于.net core 依赖注入的方式添加服务,方便快捷；  
特性sql,基于`model`上添加`特性`的方式，通过特定的扩展方式将特性的配置转换成sql语句，便于查询的扩展以及查询条件的动态绑定；  
Support Db：Mysql、Sqlserver、Oracle
## Advantage
* 动态拼接where部分，不需要在服务层编写重复性的代码，Dto端配置好，服务端只需要一句话即可完成各种复杂的查询；
* 展示给前端的字段与数据库实际配置字段分开，可任意起别名，安全性高；
* sql查询部分自动缓存，只要服务不重启，同一接口的查询从缓存读取sql，效率高；
* select字段或者where部分字段的添加或删除便捷，只需要修改Dto模型即可，对表结构变化以及查询多样化兼容性好；
* 配合Swagger文档使用，方便文档查阅；
## New Function Description
* Add demo
* Add oracle Database Support
* sqlserve Repair of vulnerabilities
* Add Having Attribute Support
## Demo
### demo version core3.1
服务层ConfigureServices简单一句话即可完成参数注入：
``` c#
services.AddAttributeSqlService(option =>  
{  
    option.UseMysql(connStr);  
});  
``` 
### Dto模型配置：
#### where部分
OperationCode为该字段操作符，TableByName为表别名，DbFieldName为数据库字段名
``` c#
/// <summary>
/// where部分
/// </summary>
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
``` 
#### select部分
MainTable 主表即sql from 后面的表，LeftTable为左连接表，TableByName表别名，DbFieldName数据库字段名
``` c#
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
    [TableByName("p")]
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
    [DbFieldName("(select R03_PayAmount from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
    public double? PayAmount { get; set; }
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
    [DbFieldName("(select R03_PayStatus from R03_PayRecord where R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC limit 1)")]
    public int R03_PayStatus { get; set; }
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
``` 
#### 服务层调用
``` c#
/// <summary>
/// 构造函数将服务注入
/// </summary>
public DemoController(IAttrSqlClient _client)
{
    client = _client;
}
[HttpPost]
public async Task<AttrResultModel> OrderQuery([FromBody] OrderPageSearch pageSearch)
{
    //pageSearch.Index = 1;
    //pageSearch.Size = 10;
    //对于查询而言,可不创建实体,可以直接通过DebugQuerySql来获取最终生成的sql文本
    var result = AttrResultModel.Success();
    string sql = client.DebugQuerySql<OrderSearchResultDto,OrderPageSearch>(pageSearch);
    try
    {
        //调用按照Dto模型的方式查询
        result = await client.GetSpecifyResultDto<OrderPageSearch, OrderSearchResultDto>(pageSearch);                
        //若需要遍历查询结果
        var list = ((AttrPageResult<OrderSearchResultDto>)result.Result).Rows.ToList();
    }
    catch (Exception ex)
    {
        result.Code = ResultCode.UnknownError;
        result.Msg = ex.Message;
    }
    return result;
}
```
#### 最终生成的sql语句
``` sql
SELECT
	p.C02_CustomerId AS CustomerId,
	( SELECT R03_PayRecordNo FROM R03_PayRecord WHERE R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC LIMIT 1 ) AS OrderPayNo,
	( SELECT R03_PayMode FROM R03_PayRecord WHERE R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC LIMIT 1 ) AS PayMode,
	( SELECT R03_PayAccount FROM R03_PayRecord WHERE R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC LIMIT 1 ) AS PayAccount,
	o.R02_Title AS Title,
	o.R02_Body AS Body,
	o.R02_Amount AS Amount,
	( SELECT R03_PayAmount FROM R03_PayRecord WHERE R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC LIMIT 1 ) AS PayAmount,
	p.P01_ProductId AS ProductId,
	o.P02_ProductFlowId AS ProductFlowId,
	o.R02_PayStatus AS PayStatus,
	( SELECT R03_PayStatus FROM R03_PayRecord WHERE R03_PayRecord.R02_OrderPayId = o.R02_OrderPayId ORDER BY R03_CreateTime DESC LIMIT 1 ) AS R03_PayStatus,
	o.R02_OrderPayId,
	o.R02_FinishTime AS FinishTime,
	p.R01_Account AS Account 
FROM
	R01_Order p
	LEFT JOIN R02_OrderPay o ON o.R01_OrderId = p.R01_OrderId 
WHERE
	1 = 1 
	AND FIND_IN_SET( p.dbfieldname_CustomerId, @Customer ) 
ORDER BY
	p.C02_CustomerId DESC
``` 
## Other Functions 
#### delete
```c#
//根据主键删除数据（默认实体内第一个带ID的字段为主键,且该字段必须有值）
Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "");
//软删除,需要指定软删除的字段,以及然删除字段的无效值
Task<AttrResultModel> SoftDeleteAsync<TEntity>(TEntity entity, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true, string ErrorMsg = "") where TEntity : AttrEntityBase, new();
```
#### update
```c#
//更新实体 按照Dto模型中有值的部分就行更新,没有值的部分将忽略,适用于表单提交类更新
Task<AttrResultModel> UpdateHasValueFieldAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "", string PrimaryKey = "", bool IgnorIntDefault = true, bool UpdateByKey = true) where TDto : AttrBaseModel where TEntity : AttrEntityBase, new();
```
#### insert
```c#
//新增实体
Task<AttrResultModel> InsertAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "")where TDto : AttrBaseModel where TEntity : AttrEntityBase, new();
//新增实体并返回自增的主键
Task<AttrResultModel> InsertReturnKey<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "") where TDto : AttrBaseModel where TEntity : AttrEntityBase, new();
//高效批量新增
Task<AttrResultModel> NonPatameterBatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg = "") where TEntity : AttrEntityBase, new();
```
#### transaction
```c#
//事务执行
Task<AttrResultModel> TransactionRun(Func<Task<AttrResultModel>> func);
```
## Contact
QQ：648808699
Welcome Message

