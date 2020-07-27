using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AttributeSqlDLL.Core.ServiceDI;
using System.Data.Common;

namespace AttributeSql
{
    public class Startup
    {
        private IServiceCollection _services;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();
            //使用简单,将类库引用到项目中,添加服务即可
            //var connStr = "Server=127.0.0.1; Port=3306;Stmt=; Database=AttrDemoDb; Uid=root; Pwd=123456;Old Guids=true;charset=utf8;Allow User Variables=True;Convert Zero Datetime=True;";
            var connStr = $"Server=120.27.141.179;Database=qingxischool;User ID=sa;Password=qwe123456;MultipleActiveResultSets=True;";
            services.AddAttributeSqlService(option =>
            {
                option.UseSqlServer(connStr);
                //option.UseMysql(connStr);
            });
            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var db = _services.BuildServiceProvider().GetServices<DbConnection>();
            if (env.EnvironmentName.ToLower().Contains("develop"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
