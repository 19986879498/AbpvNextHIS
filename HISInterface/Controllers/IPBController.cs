﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNext.HISInterface.Filters.Filters;
using AbpvNextEntityFrameworkCoreForOracle;

using HISInterface.Filters;
using HISInterface.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace HISInterface.Controllers
{
    [ServiceFilter(typeof(AbpvNextExceptionFilterAttribute))]
    [TypeFilter(typeof(AbpvNextExceptionFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class IPBController : ControllerBase
    {
        private readonly ICheckSqlConn conn;
        private readonly IZHYYDbMethods zhyyservice;
        private readonly ILoggerService logger;

        public IPBController(AbpvNextHISInterfaceDbContext dB,IConfiguration configuration, ICheckSqlConn conn,IZHYYDbMethods zhyyservice, ILoggerService logger)
        {
            this.db = dB;
            this.conn = conn;
            this.zhyyservice = zhyyservice;
            this.logger = logger;
        }

        public AbpvNextHISInterfaceDbContext db { get; set; }

        /// <summary>
        /// 测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet,Route("Get")]
        public IActionResult Get()
        {
            this.UpdateSql(SqlType: "lis");
            string sql = "select  r.lab_apply_no \"id\", r.paritemname \"title\",r.micro_flag \"iswsw\",r.patient_id \"patientId\",\'' \"patientName\",to_char(r.report_date_time,'yyyy-mm-dd hh24:mi:ss') \"sendTime\",\'枝江市人民医院' \"hospitalName\" from v_jhmk_lis_report r where r.patient_id = '0000200745' and r.is_valid = '1' order by r.report_date_time desc";
            var dt = Methods.SqlQuery(db, sql);
            ArrayList arr = Methods.getJObject(dt);


            return new ObjectResult(arr);

        }
        /// <summary>
        /// 查询检查报头
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "cardNo":"门诊卡号"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost,Route("queryInspectionReport")]
        public IActionResult queryInspectionReport([FromBody] dynamic dy)
        {
            this.UpdateSql("lis");
            JObject res = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\nqueryInspectionReport的入参" + res.ToString());
            #region 老版本屏蔽
            //string patientno = string.Empty;
            //try
            //{
            //    patientno = res.GetValue("cardNo").ToString();
            //}
            //catch 
            //{
            //    return new JsonResult(new { msg = "你输入的参数有误！", data = "参数错误", code = 403 });
            //}
            //string sql = $"select  r.lab_apply_no \"id\", r.paritemname \"title\",r.micro_flag \"iswsw\",r.patient_id \"patientId\",\'' \"patientName\",to_char(r.report_date_time,'yyyy-mm-dd hh24:mi:ss') \"sendTime\",\'枝江市人民医院' \"hospitalName\" from v_lis_report_zhyy r where   (r.patient_id='{patientno}' and r.file_visit_type='2') or (r.patient_id = '{patientno}' and r.file_visit_type = '0') and r.is_valid = '1' order by r.report_date_time desc";
            //var dt = Methods.SqlQuery(db, sql);
            //ArrayList arr = Methods.getJObject(dt);
            //if (arr.Count==0||arr==null)
            //{
            //    return new JsonResult(new { msg = "没有找到如何检测报告的信息！", data ="查询结果为空", code = 404 });
            //} 
            #endregion
            IActionResult result= this.zhyyservice.BindServiceForSql(db, "queryInspectionReport","IPB", res);
            return result;
        }
        /// <summary>
        /// 检查详情查询
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "id":"检查id",
        ///      "iswsw":"是否微生物 1 是 0否"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost,Route("queryInspectionReportDetails")]
        public IActionResult queryInspectionReportDetails([FromBody ] dynamic dy)
        {
            this.UpdateSql("lis");
            JObject jObject = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\nLIS接口的入参" + jObject.ToString());
            string id = string.Empty;
            string iswsw = string.Empty;
            try
            {
                 id = jObject.GetValue("id").ToString();
                 iswsw = jObject.GetValue("iswsw").ToString();
            }
            catch 
            {
                return new JsonResult(new { msg = "你输入的参数有误！", data = "参数错误", code = 403 });
            }
            string sql = string.Empty;
            IActionResult res = new JsonResult("");
            if (iswsw=="0")
            {
                res = this.zhyyservice.BindServiceForSql(db, "queryInspectionReportDetailsPT", "IPB", jObject);
            }
            else if (iswsw == "1")
            {
                res = this.zhyyservice.BindServiceForSql(db, "queryInspectionReportDetailsWSW", "IPB", jObject);
            }
            else
            {
                res= new JsonResult(new { msg = "iswsw参数找不到对应值！", data = "参数错误", code = 403 });
            }
            return res;
            #region 老版本屏蔽
            //var dt = Methods.SqlQuery(db, sql);
            //ArrayList arr = Methods.getJObject(dt);
            //if (arr.Count == 0 || arr == null)
            //{
            //    return new JsonResult(new { msg = "没有找到如何检测报告详情的信息！", data = "查询结果为空", code = 404 });
            //}
            //if (iswsw=="1")
            //{
            //    IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(arr));
            //    string[] ids = j.Select(s => s.GetValue("itemName").ToString()).Distinct().ToArray();
            //    List<object> objects = new List<object>();
            //    foreach (string item in ids)
            //    {
            //        objects.Add(new JsonResult(new { item = j.FirstOrDefault(u => u.GetValue("itemName").ToString() == item).GetValue("item").ToString(), itemName =item.ToString(), details = (from JObject x in j where x.GetValue("itemName").ToString() == item select x).Select(s => new { name = s.GetValue("name").ToString(), value = s.GetValue("value").ToString(), reference = s.GetValue("reference").ToString(), unit = s.GetValue("unit").ToString(), status = s.GetValue("status").ToString(), remark = s.GetValue("remark").ToString() }) }).Value);
            //    }
            //    return new JsonResult(new { msg = "查询成功！", data = objects, code = 200 });
            //}
            //else
            //{
            //    return new JsonResult(new { msg = "查询成功！", data = arr, code = 200 });
            //} 
            #endregion

        }
        #region 查询检验报告
        /// <summary>
        /// 查询检验报告
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "cardNo":"门诊卡号"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("queryMedicalReport")]
        public IActionResult queryMedicalReport([FromBody] dynamic dy)
        {
            this.UpdateSql("pacs");
            JObject jobj = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\nPACS的入参" + jobj.ToString());

            IActionResult result = this.zhyyservice.BindServiceForSql(db, "queryMedicalReport", "IPB", jobj);
            return result;

        } 
        #endregion

        #region 住院预交金收取接口
        /// <summary>
        /// 住院预交金收取接口PRC_InPrepayPayedConfirm
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "InpatientNo":"住院流水号",
        ///      "IdCard":"身份证",
        ///      "TransNo":"交易流水",
        ///      "YJCost":"预交金额",
        ///      "YJTime":"预交时间",
        ///      "PayMode":"支付方式"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost,Route("PayInPrepayCost")]
        public IActionResult PayInPrepayCost([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n住院预交金收取接口的入参" + j.ToString());
            #region 老版本屏蔽
            //List<OracleParameter> oralist = new List<OracleParameter>();
            //#region 绑定存储过程的参数
            //try
            //{
            //    oralist.Add(Methods.GetInput("InpatientNo", j.GetValue("InpatientNo").ToString()));
            //    oralist.Add(Methods.GetInput("IdCard", j.GetValue("IdCard").ToString()));
            //    oralist.Add(Methods.GetInput("TransNo", j.GetValue("TransNo").ToString()));
            //    oralist.Add(new OracleParameter() { ParameterName = "YJCost", OracleDbType = OracleDbType.Decimal, Value = Convert.ToDecimal(j.GetValue("YJCost").ToString()) });
            //    oralist.Add(new OracleParameter() { ParameterName = "YJTime", OracleDbType = OracleDbType.Date, Value = Convert.ToDateTime(j.GetValue("YJTime").ToString()) });
            //    oralist.Add(Methods.GetInput("PayMode", j.GetValue("PayMode").ToString()));
            //}
            //catch (Exception)
            //{
            //    return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            //}
            //oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            //oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            //oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            //#endregion
            //var ds = Methods.SqlQuery(db, "zjhis.PKG_ZHYY_MZ.PRC_InPrepayPayedConfirm", oralist.ToArray()); 
            #endregion
            IActionResult resobj = zhyyservice.BindService(db, "PayInPrepayCost", "IPB",j);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj));
            return resobj;
        }
        #endregion

        /// <summary>
        /// 切换数据库的方法
        /// </summary>
        /// <param name="SqlType"></param>
        private void UpdateSql(string SqlType)
        {
            switch (SqlType.ToUpper())
            {
                case "HIS":
                    this.db = conn.GetHISDBZSK(db);
                    break;
                case "LIS":
                    this.db = conn.GetLISDB(db);
                    break;
                case "PACS":
                    this.db = conn.GetPacsDB(db);
                    break;
                default:
                    this.db = db;
                    break;
            }
        }
    }
}