using AbpvNextEntityFrameworkCoreForOracle;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbpvNext.Application.HISInterface.IDBFactory
{
   public interface ICheckSqlConn
    {
        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public AbpvNextHISInterfaceDbContext GetHISDBCSK(AbpvNextHISInterfaceDbContext dB);
        /// <summary>
        /// 连接HIS数据库测试库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public AbpvNextHISInterfaceDbContext GetHISDBCSK2(AbpvNextHISInterfaceDbContext dB);
        /// <summary>
        /// 连接HIS数据库正式库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public AbpvNextHISInterfaceDbContext GetHISDBZSK(AbpvNextHISInterfaceDbContext dB);
        /// <summary>
        /// 连接LIS数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public AbpvNextHISInterfaceDbContext GetLISDB(AbpvNextHISInterfaceDbContext dB);
        /// <summary>
        /// 连接Pacs数据库
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public AbpvNextHISInterfaceDbContext GetPacsDB(AbpvNextHISInterfaceDbContext dB);
    }
}
