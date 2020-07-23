using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSql.Entity;
using AttributeSql.Model;
using AttributeSqlDLL.Core.IService;
using AttributeSqlDLL.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace AttributeSql.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private IAttrSqlClient client { get; set; }
        public DemoController(IAttrSqlClient _client)
        {
            client = _client;
        }
        /// <summary>
        /// 多表关联查询demo
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AttrResultModel> OrderQuery([FromBody] OrderPageSearch pageSearch)
        {
            //pageSearch.Index = 1;
            //pageSearch.Size = 10;
            //对于查询而言,可不创建实体,可以直接通过DebugQuerySql来获取最终生成的sql文本
            string sql = client.DebugQuerySql<OrderSearchResultDto,OrderPageSearch>(pageSearch);
            var result = await client.GetSpecifyResultDto<OrderPageSearch, OrderSearchResultDto>(pageSearch);
            //若需要遍历查询结果
            var list = ((AttrPageResult<OrderSearchResultDto>)result.Result).Rows.ToList();
            return result;
        }
        /// <summary>
        /// 新增操作
        /// </summary>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AttrResultModel> InsertOrder([FromBody] CreateOrderDto orderDto)
        {
            //事务新增,如果任一操作失败,都会回滚
            var result = await client.TransactionRun(async () =>
            {
                //模型与实体的关系需要在UserProfile建立映射
                var serverResult = await client.InsertReturnKey<CreateOrderDto, R01_Order>(orderDto, "新增出错");
                //建议在执行下一次操作之前判断上一个执行的状态是否是成功
                if (serverResult.Code == 0)
                {
                    R02_OrderPay orderPay = new R02_OrderPay()
                    {
                        R02_OrderPayNo = "编号",
                        R01_OrderId = (int)serverResult.Result,
                        R01_OrderNo = orderDto.R01_OrderNo,
                        P02_ProductFlowId = orderDto.P02_ProductFlowId,
                        R02_Title = orderDto.Title,
                        R02_Body = orderDto.Body,
                        R02_Amount = orderDto.Amount,
                        R02_PayStatus = 1,
                        R02_IsValid = 1,
                        R02_CreateId = (long)orderDto.R01_CreateId,
                        R02_CreateBy = orderDto.R01_CreateBy
                    };
                    serverResult = await client.InsertEntityAsync(orderPay);
                }
                //最终会根据执行的状态来判断回滚还是提交
                return serverResult;
            });
            return result;
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AttrResultModel> UpdateOrder([FromBody] CreateOrderDto orderDto)
        {
            //这里如果不指定主键的话，默认会按照实体的第一个带有ID的字段为主键,建议手动指定
            var result = await client.UpdateHasValueFieldAsync<CreateOrderDto, R01_Order>(orderDto,"更新出错", "R01_OrderId");
            result = await client.UpdateAsync<CreateOrderDto, R01_Order>(orderDto, "更新出错", "R01_OrderId");
            result = await client.ExecUpdateSql<R01_Order>(null,"Update R01_Order set R01_OrderNo = 'test' where R01_OrderId = 1","更新出错");
            return result;                     
        }
        /// <summary>
        /// 刪除操作
        /// </summary>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AttrResultModel> DeleteOrder([FromBody] CreateOrderDto orderDto)
        {
            var result = await client.TransactionRun(async () =>
            {
                //直接删除
                var serverResult = await client.DeleteAsync<CreateOrderDto, R01_Order>(orderDto,new string[] { "R01_OrderNo" }, "删除出错");
                //软删除
                serverResult = await client.SoftDeleteAsync<CreateOrderDto, R01_Order>(orderDto,"R01_IsValid", 2, "R01_OrderId");
                return serverResult;
            });
            return result;
        }
    }
}
