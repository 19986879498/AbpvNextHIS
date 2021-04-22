using AbpvNext.Application.HISInterface.DBFactory;
using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNextEntityFrameworkCoreForOracle;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace AbpvNext.Application.HISInterface
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpvNextEntityFrameworkCoreForOracleModule)
        )]
    public class AbpvNextApplicationModule:AbpModule
    {
        //log4net日志
        public static ILoggerRepository repository { get; set; }
        public AbpvNextApplicationModule()
        {
            //加载log4net日志配置文件
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //注入AutoMapper
            Configure<AbpAutoMapperOptions>(opt => {
                opt.AddProfile<AbpvNextApplicationProfile>();
            });
            Configure<ILog>(opt =>
            {

            });
            //依赖注入
            var services = context.Services;
            services.AddTransient<ICheckSqlConn, CheckSqlConn>();
            services.AddTransient<IFunService, FunService>();
            services.AddTransient<IZHYYDbMethods, ZHYYDbMethods>();
            services.AddTransient<ILoggerService, LoggerService>();
            base.ConfigureServices(context);
        }
    }
}
