using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using Volo.Abp.Dapper;
using Volo.Abp.Modularity;

namespace AbpvNext.DapperCore.HISInterface
{
    [DependsOn(
        typeof(AbpDapperModule)
        )]
    public class AbpvNextDapperModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            context.Services.AddSingleton<IDbConnection, OracleConnection>(opt=> {
                return new OracleConnection(root["OrclDBStrCSK2"].ToString());
            });
            base.ConfigureServices(context);
        }
    }
}
