using AbpvNext.Application.HISInterface;
using AbpvNext.DapperCore.HISInterface;
using AbpvNext.HISInterface.Filters;
using AbpvNextEntityFrameworkCoreForOracle;
using Castle.Core.Configuration;
using HISInterface.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HISInterface
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AbpvNextEntityFrameworkCoreForOracleModule),
        typeof(AbpvNextApplicationModule),
        typeof(AbpvNextDapperModule),
        typeof(AbpvNextHISInterfaceFiltersModuls)
        )]
    public class AbpvNextHISInterfaceModule:AbpModule
    {
        private readonly IConfigurationRoot Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            #region 已通过模块化进行注册  暂时屏蔽
            //services.AddControllersWithViews(item => {
            //    item.ModelBinderProviders.Insert(0, new JObjectModelBinderProvider());
            //    item.Filters.Add(typeof(CustomExceptionFilterAttribute));
            //});
            //services.AddTransient<CustomExceptionFilterAttribute>();
            //services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();
            //services.AddDbContextPool<DBContext.DB>(db => db.UseOracle(Configuration["OrclDBStrCSK"].ToString(), item => item.UseOracleSQLCompatibility("11"))); 
            #endregion
            services.AddCors(options =>
            {
                options.AddPolicy("mycor", policy =>
                {
                    // 设定允许跨域的来源，有多个可以用','隔开
                    policy.WithOrigins("http://localhost:6500", "http://localhost:8080")//只允许https://localhost:6500来源允许跨域
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HISInterface", Version = "v1" });
                // 为 Swagger JSON and UI设置xml文档注释路径

                var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //添加swagger注释
                var xmlPath = Path.Combine(basePath, "HISInterface.xml");
                c.IncludeXmlComments(xmlPath);
            });
            base.ConfigureServices(context);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            ILoggerFactory loggerFactor =  context.GetLoggerFactory();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ////加载Nlog的nlog.config配置文件
            //LogManager.LoadConfiguration("Config/nlog.config");
            ////添加NLog
            //loggerFactor.AddNLog(); 
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HISInterface v1"));
            //  app.UseCors("CustomCorsPolicy");
            // app.UseHttpsRedirection();
            app.UseCors("mycor");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            base.OnApplicationInitialization(context);
        }
    }
}
