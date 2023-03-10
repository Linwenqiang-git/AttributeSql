using AttributeSql.Base.Models;
using AttributeSql.Core;
using AttributeSql.Core.Extensions;
using AttributeSql.Core.Repository;
using AttributeSql.Core.Services;
using AttributeSql.Demo.DbContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

using System.Collections.Generic;
using System.IO;
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
            //根据自己的数据库添加对应的执行器
            context.Services.AddPgsqlExecutorService();
            //注入特性sql服务
            context.Services.AddTransient(typeof(IAttrSqlService<>), typeof(AttrSqlService<>));
            ConfigureSwaggerServices(context.Services);
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
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SCM CmsCenter API");
            });

        }
        private void ConfigureSwaggerServices(IServiceCollection service)
        {
            service.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SCM CmsCenter API", Version = "v0.1" });
                options.DocInclusionPredicate((doc, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                foreach (var item in XmlCommentsFilePath)
                {
                    options.IncludeXmlComments(item, true);
                }
                options.SchemaFilter<EnumSchemaFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                            },
                            new string[] { }
                        },
                });
            });
        }
        private List<string> XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                DirectoryInfo d = new DirectoryInfo(basePath);
                FileInfo[] files = d.GetFiles("*.xml");
                var xmls = files.Select(a => Path.Combine(basePath, a.FullName)).ToList();
                return xmls;
            }
        }
    }
}
