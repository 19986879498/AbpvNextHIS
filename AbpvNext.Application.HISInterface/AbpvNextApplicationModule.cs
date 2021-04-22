using AbpvNext.Application.HISInterface.DBFactory;
using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNextEntityFrameworkCoreForOracle;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //注入AutoMapper
            Configure<AbpAutoMapperOptions>(opt => {
                opt.AddProfile<AbpvNextApplicationProfile>();
            });
            //依赖注入
            var services = context.Services;
            services.AddTransient<ICheckSqlConn, CheckSqlConn>();
            services.AddTransient<IFunService, FunService>();
            services.AddTransient<IZHYYDbMethods, ZHYYDbMethods>();
            base.ConfigureServices(context);
        }
    }
}
