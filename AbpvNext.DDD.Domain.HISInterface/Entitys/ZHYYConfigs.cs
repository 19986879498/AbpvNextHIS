using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace AbpvNext.DDD.Domain.HISInterface.Entitys
{
   public class COM_ZHYY_CONFIG
    {
        /// <summary>
        /// 主键配置Id
        /// </summary>
        //[Column("CONFIGID")]
        [Key]
        public string CONFIGID { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
      // [Column("SERVICENAME")]
        public string SERVICENAME { get; set; }
        /// <summary>
        /// 服务模块
        /// </summary>
        //[Column("SERVICEMODULE")]
        public string SERVICEMODULE { get; set; }
        /// <summary>
        /// 存储过程名称
        /// </summary>
       //[Column("PROCEDURENAME")]
        public string PROCEDURENAME { get; set; }
        /// <summary>
        /// 入参的内容可填多个 使用|隔开
        /// </summary>
       // [Column("INPARAMETERNAMES")]
        public string INPARAMETERNAMES { get; set; }
        /// <summary>
        /// 入参类型可用|隔开
        /// </summary>
        //[Column("INPARAMETERTYPES")]
        public string INPARAMETERTYPES { get; set; }
        /// <summary>
        /// 出参的内容可填多个 使用|隔开
        /// </summary>
        //[Column("OUTPARAMETERNAMES")]
        public string OUTPARAMETERNAMES { get; set; }
        /// <summary>
        /// 出参类型可用|隔开
        /// </summary>
       // [Column("OUTPARAMETERTYPES")]
        public string OUTPARAMETERTYPES { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        //[Column("REMARK")]
        public string REMARK { get; set; }
        /// <summary>
        /// 存储过程的入参
        /// </summary>
        public string INPROCEDUREPROP { get; set; }
        /// <summary>
        /// 是否分页 1 分页 2 不分页
        /// </summary>
        public string ISFY { get; set; }
        /// <summary>
        /// 是否有效 1 有效 2 无效
        /// </summary>
        public string VALID_FALG { get; set; }
        /// <summary>
        /// api是否二级分组
        /// </summary>
        public string ISSONSHOW { get; set; }

    }
}
