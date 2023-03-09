using AttributeSql.Base.Models;
using AttributeSql.Core;
using AttributeSql.Core.Extensions;
using AttributeSql.Core.Repository;
using AttributeSql.Core.Services;
using AttributeSql.Demo.DbContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Net.Http;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AttributeSql
{
    [DependsOn(typeof(AttributeSqlCoreModule))]
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    [DependsOn(typeof(AbpAutofacModule))]
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class AttributeSqlDemoModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<MvcOptions>(options =>
            {
                var filterMetadata = options.Filters.FirstOrDefault(x => x is ServiceFilterAttribute attribute && attribute.ServiceType == typeof(AbpExceptionFilter));
                options.Filters.Remove(filterMetadata);                
            });
            context.Services.AddAbpDbContext<AttributeSqlDemoDbContext>(options =>
            {
                options.AddDefaultRepositories(true);
            });
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(AttributeSqlDemoModule).Assembly, opts =>
                {
                    opts.RootPath = "AttrCenter";
                });
            });
            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidateIgnoredHttpMethods.Add(HttpMethod.Post.ToString());
                options.AutoValidateIgnoredHttpMethods.Add(HttpMethod.Put.ToString());
                options.AutoValidateIgnoredHttpMethods.Add(HttpMethod.Delete.ToString());
            });
            Configure<AbpDbContextOptions>(options =>
            {
                options.UseNpgsql();
            });            
            //context.Services.AddTransient(typeof(AttributeSqlDemoDbContext));
            
            context.Services.UsePgsql();
            context.Services.AddTransient(typeof(IAttrSqlService<>), typeof(AttrSqlService<>));
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            // 路由
            app.UseRouting();

            // 跨域
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            // 路由映射
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
