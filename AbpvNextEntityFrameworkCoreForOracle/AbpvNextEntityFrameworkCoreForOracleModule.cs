using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AbpvNextEntityFrameworkCoreForOracle
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpvNextEntityFrameworkCoreForOracleModule:AbpModule
    {
        //private readonly IConfigurationRoot Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpvNextHISInterfaceDbContext>();
            Configure<AbpDbContextOptions>(opt => {
                opt.UseOracle(b => b.UseOracleSQLCompatibility("11"));
            });
            base.ConfigureServices(context);
        }
    }
}
