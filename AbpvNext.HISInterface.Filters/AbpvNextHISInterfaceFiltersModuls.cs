using AbpvNext.HISInterface.Filters.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;

namespace AbpvNext.HISInterface.Filters
{
    [DependsOn(
        )]
    public class AbpvNextHISInterfaceFiltersModuls:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            //注册Filter
            services.AddSingleton<AbpvNextExceptionFilterAttribute>();
            services.AddSingleton<AbpvNextResoureFilterAttribute>();
            services.AddControllersWithViews(opt=> {
                opt.Filters.Add(typeof(AbpvNextExceptionFilterAttribute));
                opt.Filters.Add(typeof(AbpvNextResoureFilterAttribute));
            }).AddNewtonsoftJson();
            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();
           
            base.ConfigureServices(context);
        }
    }
}
