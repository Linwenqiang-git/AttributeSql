using AttributeSql.Core.Data;
using AttributeSql.Core.Models;
using AttributeSql.Core.Services;
using AttributeSql.Demo.DbContext;
using AttributeSql.Demo.Dtos;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Application.Services;

namespace AttributeSql.Controllers
{
    public class DemoController : ApplicationService
    {           
        private IAttrSqlService<AttributeSqlDemoDbContext> _client { get; set; }
        private IAttrSqlWithAbpDataFilter _abpDataFilter { get; set; }
        public DemoController(IAttrSqlService<AttributeSqlDemoDbContext> client, IAttrSqlWithAbpDataFilter abpDataFilter)
        {
            _client = client;
            _abpDataFilter = abpDataFilter;
        }
        [HttpPost]
        public async Task<AttrResultModel> query(ArticlePageSearch search)
        {
            //var result = await _client.GetSpecifyResultDto<ArticlePageSearch, ArticleResult>(search);
            //return result;
            //过滤写法
            using (_abpDataFilter.Disable<ISoftDelete>())
            {
                var filterResult = await _client.GetSpecifyResultDto<ArticlePageSearch, ArticleResult>(search);
                return filterResult;
            }
        }       
    }
}
