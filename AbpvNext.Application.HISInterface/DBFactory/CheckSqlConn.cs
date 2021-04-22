using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNextEntityFrameworkCoreForOracle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbpvNext.Application.HISInterface.DBFactory
{
   public class CheckSqlConn:ICheckSqlConn
    {
        public  IConfigurationRoot Root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public  AbpvNextHISInterfaceDbContext GetHISDBCSK(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = Root["OrclDBStrCSK"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public  AbpvNextHISInterfaceDbContext GetHISDBCSK2(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = Root["OrclDBStrCSK2"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接HIS数据库正式库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public  AbpvNextHISInterfaceDbContext GetHISDBZSK(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = Root["OrclDBStrZSK"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接LIS数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public  AbpvNextHISInterfaceDbContext GetLISDB(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = Root["OrclDBLis"].ToString();
            return dB;
        }
        /// <summary>
        /// 连接Pacs数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public  AbpvNextHISInterfaceDbContext GetPacsDB(AbpvNextHISInterfaceDbContext dB)
        {

            dB.Database.GetDbConnection().ConnectionString = Root["OrclDBPacs"].ToString();
            return dB;
        }
    }
}
