﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNext.HISInterface.Filters.Filters;
using AbpvNextEntityFrameworkCoreForOracle;
using HISInterface.Filters;
using HISInterface.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class OPBtestController : ControllerBase
    {
        private readonly ICheckSqlConn conn;

        public OPBtestController(AbpvNextHISInterfaceDbContext db,IConfiguration configuration, ICheckSqlConn conn)
        {
            this.db = db;
            this.configuration = configuration;
            this.conn = conn;
        }

        public AbpvNextHISInterfaceDbContext db { get; set; }
        public IConfiguration configuration { get; }

        /// <summary>
        /// 用于测试
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("test")]
        public IActionResult test()
        {
            //更新sql
            return Content("测试");
        }
        #region 注册就诊卡
        /// <summary>
        /// 注册就诊卡prc_outppatmedcardsell
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "idCardNo":"身份证",
        ///      "patientName":"患者姓名",
        ///      "sex":"性别",
        ///      "birthday":"生日",
        ///      "address":"住址",
        ///      "phone":"电话"
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("registCard")]
        public IActionResult registCard([FromBody] dynamic dynamic)//直接点参数
        {
            UpdateSql("HIS");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊注册就诊卡的入参" + j.ToString());
            List<OracleParameter> parems = new List<OracleParameter>();
            #region 赋值oracleParameter
            try
            {
                parems = new List<OracleParameter>() {
                Logic.Methods.GetInput("idCardNo",  j.GetValue("idCardNo").ToString()),
                Logic.Methods.GetInput("patientName", j.GetValue("patientName").ToString()),
                Logic.Methods.GetInput("sex", j.GetValue("sex").ToString()),
                 Logic.Methods.GetInput("birthday", j.GetValue("birthday").ToString()),
                Logic.Methods.GetInput("address", j.GetValue("address").ToString()),
                Logic.Methods.GetInput("phone", j.GetValue("phone").ToString()),
                 Logic.Methods.GetInput("mz", ""),
                 Logic.Methods.GetOutput("data", OracleDbType.RefCursor, 1024),
                Logic.Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20),
                Logic.Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200)
            };
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "请求失败", data = "请检查您所传递的参数是否有误！", code = "500" });
            }
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.prc_outppatmedcardsell", parems.ToArray());
            ObjectResult resobj = Methods.GetResult(parems, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion



        #region 绑定就诊卡
        /// <summary>
        /// 绑定就诊卡PRC_OutpPatMedCardCheck
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "cardType":"卡类别",
        ///      "cardWord":"卡号"
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("bindCard")]
        public IActionResult bindCard([FromBody] dynamic dynamic)//直接点参数
        {
            UpdateSql("HIS");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n绑定就诊卡入参" + j.ToString());
            #region 赋值oracleParameter
            List<OracleParameter> parems = new List<OracleParameter>();
            try
            {
                parems = new List<OracleParameter>() {
                Logic.Methods.GetInput("cardType", j.GetValue("cardType").ToString()),
                Logic.Methods.GetInput("cardWord", j.GetValue("cardWord").ToString()),
                 Logic.Methods.GetOutput("ReturnCode", OracleDbType.Int32, 50),
                Logic.Methods.GetOutput("data", OracleDbType.RefCursor, 1024),
                Logic.Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200)
            };
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "请求失败", data = "请检查您所传递的参数是否有误！", code = "500" });
            }
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpPatMedCardCheck", parems.ToArray());
            ObjectResult resobj = Methods.GetResult(parems, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            //输出参数
            return resobj;
        }
        #endregion



        #region 锁号操作
        /// <summary>
        /// 锁号的接口PRC_OutpRegisterLock
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "patientId":"门诊卡号",
        ///      "appointmentType":"",
        ///      "shemaId":"排班id",
        ///      "poolId":"号源id",
        ///      "doctorId":"医生id",
        ///      "deptName":"科室",
        ///      "appointmentOrder":"",
        ///      "lockTime":"锁号时间",
        ///      "lockState":"锁号状态"
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("appointment")]
        public IActionResult appointment([FromBody] dynamic dynamic)
        {
            //更新sql
            this.UpdateSql("HIS");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n锁号接口请求数据" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 赋值参数
            try
            {
                oralist.Add(Methods.GetInput("patientId", j.GetValue("patientId").ToString()));
                oralist.Add(Methods.GetInput("appointmentType", j.GetValue("appointmentType").ToString()));
                oralist.Add(Methods.GetInput("shemaId", j.GetValue("shemaId").ToString()));
                oralist.Add(Methods.GetInput("poolId", j.GetValue("poolId").ToString()));
                oralist.Add(Methods.GetInput("doctorId", j.GetValue("doctorId").ToString()));
                oralist.Add(Methods.GetInput("deptName", j.GetValue("deptName").ToString()));
                oralist.Add(Methods.GetInput("appointmentOrder", j.GetValue("appointmentOrder").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "appointmentTime", OracleDbType = OracleDbType.Date, Value = DateTime.Parse(j.GetValue("appointmentTime").ToString()) });
                oralist.Add(new OracleParameter() { ParameterName = "lockTime", OracleDbType = OracleDbType.Date, Value = DateTime.Parse(j.GetValue("lockTime").ToString()) });

                oralist.Add(Methods.GetInput("lockState", j.GetValue("lockState").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }
            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 2024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 50));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 50));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpRegisterLock", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion

        #region 挂号确认
        /// <summary>
        /// 挂号确认的接口PRC_OutpRegisterConfirm
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "appointmentId":"门诊卡号",
        ///      "appointmentStatus":"",
        ///      "payStatus":"支付状态",
        ///      "total":"挂号总金额",
        ///      "payType":"支付方式"
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("updateAppointmentStatus")]
        public IActionResult updateAppointmentStatus([FromBody] dynamic dynamic)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            //更新sql
            this.UpdateSql("HIS");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n挂号确认入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("appointmentId", j.GetValue("appointmentId").ToString()));
                oralist.Add(Methods.GetInput("appointmentStatus", j.GetValue("appointmentStatus").ToString()));
                oralist.Add(Methods.GetInput("payStatus", j.GetValue("payStatus").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "total", OracleDbType = OracleDbType.Decimal, Value = j.GetValue("total").ToString() });
                oralist.Add(Methods.GetInput("payType", j.GetValue("payType").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }

            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpRegisterConfirm", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            st.Stop();
            var time = st.ElapsedMilliseconds;
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion


        #region 门诊缴费操作
        /// <summary>
        /// 门诊缴费操作PRC_OutpBillsPayedConfirm
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "billId":"门诊流水号",
        ///      "OrderNo":"订单号可进行多个拼接  例如 1111|2222|3333",
        ///      "status":"支付状态",
        ///      "billType":"",
        ///      "payType":"支付方式",
        ///      "ZFAmount":"自费金额 （结算的总金额）",
        ///      "YBZHAmount":"医保账户支付金额（由于没有医保支付直接给0）",
        ///      "YBTCAmount":"医保统筹支付金额（由于没有医保支付直接给0）",
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("updateBillStatus")]
        public IActionResult updateBillStatus([FromBody] dynamic dynamic)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊缴费的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            try
            {
                oralist.Add(Methods.GetInput("billId", j.GetValue("billId").ToString()));
                oralist.Add(Methods.GetInput("OrderNo", j.GetValue("OrderNo").ToString()));
                oralist.Add(Methods.GetInput("status", j.GetValue("status").ToString()));
                oralist.Add(Methods.GetInput("billType", j.GetValue("billType").ToString()));
                oralist.Add(Methods.GetInput("payType", j.GetValue("payType").ToString()));
                oralist.Add(Methods.GetInput("ZFAmount", j.GetValue("ZFAmount").ToString()));
                oralist.Add(Methods.GetInput("YBZHAmount", j.GetValue("YBZHAmount").ToString()));
                oralist.Add(Methods.GetInput("YBTCAmount", j.GetValue("YBTCAmount").ToString()));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { msg = "请求失败", data = ex.Message, code = "500" });
            }
            oralist.Add(Methods.GetOutput("ReturnSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 50));
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpBillsPayedConfirm", oralist.ToArray());
            ObjectResult obj = Methods.GetResult(oralist, ds);
            st.Stop();
            var time = st.ElapsedMilliseconds;
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(obj.Value));
            return obj;
        }
        #endregion

        #region 挂号退号
        /// <summary>
        /// 挂号退号PRC_OutpRegisterCancel
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "tiket_no":"门诊流水号",
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("registeredBack")]
        public IActionResult registeredBack([FromBody] dynamic dynamic)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊挂号退号的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            try
            {
                oralist.Add(Methods.GetInput("ClinicNo", j.GetValue("tiket_no").ToString()));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { msg = "请求失败", data = "参数错误", code = "500" });
            }
            oralist.Add(Methods.GetOutput("ReturnSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 50));
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpRegisterCancel", oralist.ToArray());
            ObjectResult obj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(obj.Value));
            return obj;
        }
        #endregion

        #region 加号查询
        /// <summary>
        /// 加号查询PRC_RegisterAddQuery
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "CardNo":"门诊卡号",
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAddRegister")]
        public IActionResult GetAddRegister([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n加号查询的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("CardNo", j.GetValue("CardNo").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }

            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_RegisterAddQuery", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion


        #region 加号确认接口
        /// <summary>
        /// 加号确认接口PRC_RegisterAddConfirm
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "appointmentId":"门诊卡号",
        ///      "appointmentStatus":"",
        ///      "payStatus":"支付状态",
        ///      "total":"挂号总金额",
        ///      "payType":"支付方式"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("AddRegisterConfirm")]
        public IActionResult AddRegisterConfirm([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊加号确认的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("appointmentId", j.GetValue("appointmentId").ToString()));
                oralist.Add(Methods.GetInput("appointmentStatus", j.GetValue("appointmentStatus").ToString()));
                oralist.Add(Methods.GetInput("payStatus", j.GetValue("payStatus").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "total", OracleDbType = OracleDbType.Decimal, Value = j.GetValue("total").ToString() });
                oralist.Add(Methods.GetInput("payType", j.GetValue("payType").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }

            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_RegisterAddConfirm", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion



        #region 扫码加号查询
        /// <summary>
        /// 扫码加号查询PRC_RegisterSMAddQuery
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "doctorId":"医生id",
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("SMAddQuery")]
        public IActionResult SMAddQuery([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊扫码加号查询的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("DoctCode", j.GetValue("doctorId").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }

            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_RegisterSMAddQuery", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion

        #region 扫码挂号
        /// <summary>
        /// 扫码挂号PRC_OutpRegisterSMAdd
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "departId":"科室id",
        ///      "doctorId":"医生id",
        ///      "RegLevel":"挂号级别",
        ///      "CardNo":"门诊卡号",
        ///      "payStatus":"支付状态",
        ///      "payType":"支付方式",
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("SMRegister")]
        public IActionResult SMRegister([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n门诊扫码挂号的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("DepartCode", j.GetValue("departId").ToString()));
                oralist.Add(Methods.GetInput("DoctorCode", j.GetValue("doctorId").ToString()));
                oralist.Add(Methods.GetInput("RegLevel", j.GetValue("RegLevel").ToString()));
                oralist.Add(Methods.GetInput("CardNo", j.GetValue("CardNo").ToString()));
                oralist.Add(Methods.GetInput("payStatus", j.GetValue("payStatus").ToString()));
                oralist.Add(Methods.GetInput("payType", j.GetValue("payType").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "totCost", OracleDbType = OracleDbType.Decimal, Value = j.GetValue("total").ToString() });

            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }

            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OutpRegisterSMAdd", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }

        #region 其他费用缴费接口
        /// <summary>
        /// 其他费用缴费接口
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "userId":"微信用户id",
        ///      "phone":"电话",
        ///      "hospitalId":"",
        ///      "deptId":"科室id",
        ///      "projectType":"其他费用类别",
        ///      "projectPrice":"金额",
        ///      "payStatus":"支付状态",
        ///      "payType":"支付方式",
        ///      "payment":"",
        ///      "outTradeNo":"",
        ///      "wxTradeNo":"微信支付id",
        ///      "createTime":"创建时间",
        ///      "payTime":"支付时间",
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("sendOtherTypeRecord")]
        public IActionResult sendOtherTypeRecord([FromBody] dynamic dynamic)
        {
            //更新sql
            this.UpdateSql("HIS");
            JObject j = Methods.dynamicToJObject(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n其他费用缴费接口请求数据" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 赋值参数
            try
            {
                oralist.Add(Methods.GetInput("userId", j.GetValue("userId").ToString()));
                oralist.Add(Methods.GetInput("phone", j.GetValue("phone").ToString()));
                oralist.Add(Methods.GetInput("hospitalId", j.GetValue("hospitalId").ToString()));
                oralist.Add(Methods.GetInput("deptId", j.GetValue("deptId").ToString()));
                oralist.Add(Methods.GetInput("projectType", j.GetValue("projectType").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "projectPrice", OracleDbType = OracleDbType.Decimal, Value = decimal.Parse(j.GetValue("projectPrice").ToString()) });
                oralist.Add(Methods.GetInput("payStatus", j.GetValue("payStatus").ToString()));
                oralist.Add(Methods.GetInput("payType", j.GetValue("payType").ToString()));
                oralist.Add(Methods.GetInput("payment", j.GetValue("payment").ToString()));
                oralist.Add(Methods.GetInput("outTradeNo", j.GetValue("outTradeNo").ToString()));
                oralist.Add(Methods.GetInput("wxTradeNo", j.GetValue("wxTradeNo").ToString()));
                oralist.Add(new OracleParameter() { ParameterName = "createTime", OracleDbType = OracleDbType.Date, Value = DateTime.Parse(j.GetValue("createTime").ToString()) });
                oralist.Add(new OracleParameter() { ParameterName = "payTime", OracleDbType = OracleDbType.Date, Value = DateTime.Parse(j.GetValue("payTime").ToString()) });
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }
            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 2024));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 50));
            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 50));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_OtherFeePayedConfirm", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion

        #region 修改患者基本信息接口
        /// <summary>
        /// 修改患者基本信息接口
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "CardNo":"门诊卡号",
        ///      "name":"姓名",
        ///      "IdCard":"身份证",
        ///      "phone":"电话",
        ///      "address":"住址"
        ///  }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdatePatientInfo")]
        public IActionResult UpdatePatientInfo([FromBody] dynamic dy)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dy);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n修改患者基本信息接口的入参" + j.ToString());
            List<OracleParameter> oralist = new List<OracleParameter>();
            #region 绑定存储过程的参数
            try
            {
                oralist.Add(Methods.GetInput("CardNO", j.GetValue("CardNo").ToString()));
                oralist.Add(Methods.GetInput("patname", j.GetValue("name").ToString()));
                oralist.Add(Methods.GetInput("IdCard", j.GetValue("IdCard").ToString()));
                oralist.Add(Methods.GetInput("phone", j.GetValue("phone").ToString()));
                oralist.Add(Methods.GetInput("HomeAddress", j.GetValue("address").ToString()));
            }
            catch (Exception)
            {
                return new ObjectResult(new { msg = "操作失败", data = "请检查你的入参是否不一致", code = 404 });
            }
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            oralist.Add(Methods.GetOutput("ResultSet", OracleDbType.RefCursor, 1024));

            oralist.Add(Methods.GetOutput("ErrorMsg", OracleDbType.Varchar2, 200));
            #endregion
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_UpdatePatientInfo", oralist.ToArray());
            ObjectResult resobj = Methods.GetResult(oralist, ds);
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(resobj.Value));
            return resobj;
        }
        #endregion

        #endregion

        #region 预约检查新型冠状核酸检测
        /// <summary>
        /// 预约检查新型冠状核酸检测PRC_CHECKITEM_APPLY
        /// </summary>
        /// <remarks>
        /// >参数实例
        /// {
        ///      "CardNo":"门诊卡号",
        ///      "Items":["","",""],项目编号
        ///      "peopleType":"人群类别",
        ///      "date":"时间"
        ///  }
        /// </remarks>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        [HttpPost, Route("ApplyCheck")]
        public IActionResult ApplyCheck([FromBody] dynamic dynamic)
        {
            this.UpdateSql("his");
            JObject j = Methods.dynamicToJObject(dynamic);
            string ItemList = string.Empty;
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n预约检查新型冠状核酸检测的入参" + j.ToString());
            //判断参数的准确性
            if (!j.ContainsKey("CardNO"))
            {
                return new ObjectResult(new { msg = "请求失败", data = "参数错误！没有找到参数名为CardNO的参数", code = "500" });
            }
            if (!j.ContainsKey("Items"))
            {
                return new ObjectResult(new { msg = "请求失败", data = "参数错误！没有找到参数名为Items的参数", code = "500" });
            }
            else
            {
                string[] Itemslist = Methods.getStringToT<string[]>(j.GetValue("Items", StringComparison.OrdinalIgnoreCase).ToString());
                for (int i = 0; i < Itemslist.Length; i++)
                {
                    if (i == Itemslist.Length - 1)
                    {
                        ItemList += Itemslist[i];
                    }
                    else
                    {
                        ItemList += Itemslist[i] + "|";
                    }
                }
            }
            if (!j.ContainsKey("peopleType"))
            {
                return new ObjectResult(new { msg = "请求失败", data = "参数错误！没有找到参数名为peopleType的参数", code = "500" });
            }
            if (!j.ContainsKey("date"))
            {
                return new ObjectResult(new { msg = "请求失败", data = "参数错误！没有找到参数名为date的参数", code = "500" });
            }
            List<OracleParameter> oralist = new List<OracleParameter>();
            //入参
            oralist.Add(Methods.GetInput("CARD_NO", j.GetValue("CardNo", StringComparison.OrdinalIgnoreCase).ToString()));
            oralist.Add(Methods.GetInput("Item_Code", ItemList));
            oralist.Add(Methods.GetInput("people_type", j.GetValue("peopleType", StringComparison.OrdinalIgnoreCase).ToString()));
            try
            {
                oralist.Add(Methods.GetInput("PRE_DATE", OracleDbType.Varchar2, Convert.ToDateTime(j.GetValue("date", StringComparison.OrdinalIgnoreCase).ToString()).ToString("yyyy-MM-dd HH:mm:ss")));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { msg = "请求失败", data = "时间格式错误" + ex.Message, code = "500" });
            }
            //出参
            oralist.Add(Methods.GetOutput("ReturnSet", OracleDbType.RefCursor, 1024));
            oralist.Add(Methods.GetOutput("ErrStr", OracleDbType.Varchar2, 100));
            oralist.Add(Methods.GetOutput("ReturnCode", OracleDbType.Int32, 20));
            //执行
            var ds = Methods.SqlQuery(db, "PKG_ZHYY_MZ.PRC_CHECKITEM_APPLY", oralist.ToArray());
            ObjectResult obj = Methods.GetResult(oralist, ds, "ErrStr");
            Console.WriteLine("返回数据：\n" + JsonConvert.SerializeObject(obj.Value));
            return obj;
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
                    this.db = conn.GetHISDBCSK(db);
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