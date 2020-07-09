using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSql.Model;
using AttributeSqlDLL.IService;
using AttributeSqlDLL.Model;
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
        [HttpPost]
        public async Task<AttrResultModel> OrderQuery(OrderPageSearch pageSearch)
        {
            //对于查询而言,可不创建实体,可以直接通过DebugQuerySql来获取最终生成的sql文本
            string sql = client.DebugQuerySql<OrderSearchResultDto,OrderPageSearch>(pageSearch);
            var result = await client.GetSpecifyResultDto<OrderPageSearch, OrderSearchResultDto>(pageSearch);
            return result;
        }
    }
}
