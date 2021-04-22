using AbpvNext.DDD.Domain.HISInterface.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace AbpvNextEntityFrameworkCoreForOracle
{
    [ConnectionStringName("OrclDBStr")]
    public class AbpvNextHISInterfaceDbContext : AbpDbContext<AbpvNextHISInterfaceDbContext>
    {
        /// <summary>
        /// 服务配置表
        /// </summary>
        public DbSet<COM_ZHYY_CONFIG> COM_ZHYY_CONFIG { get; set; }
        public AbpvNextHISInterfaceDbContext(DbContextOptions<AbpvNextHISInterfaceDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
