using AbpvNextEntityFrameworkCoreForOracle;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISInterface.Logic
{
    public   class SqlMethods
    {
        public static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static AbpvNextHISInterfaceDbContext GetHISDBCSK(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = root["OrclDBStrCSK"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static AbpvNextHISInterfaceDbContext GetHISDBCSK2(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = root["OrclDBStrCSK2"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接HIS数据库正式库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static AbpvNextHISInterfaceDbContext GetHISDBZSK(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = root["OrclDBStrZSK"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接LIS数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static AbpvNextHISInterfaceDbContext GetLISDB(AbpvNextHISInterfaceDbContext dB)
        {

           dB.Database.GetDbConnection().ConnectionString = root["OrclDBLis"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接Pacs数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static AbpvNextHISInterfaceDbContext GetPacsDB(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = root["OrclDBPacs"].ToString();
            return dB;
        }
    }
}
