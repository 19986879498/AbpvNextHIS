using AbpvNext.DDD.Domain.HISInterface.Entitys;
using AbpvNextEntityFrameworkCoreForOracle;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AbpvNext.Application.HISInterface.IDBFactory
{
    public interface IZHYYDbMethods
    {
        /// <summary>
        /// 调用存储过程获取方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="spName"></param>
        /// <param name="paramsters"></param>
        /// <returns></returns>
        public  DataSet SqlQuery(AbpvNextHISInterfaceDbContext db, string spName, params OracleParameter[] paramsters);

        /// <summary>
        /// 获取查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  DataSet SqlQuery(AbpvNextHISInterfaceDbContext db, string sql);

        /// <summary>
        /// 查询返回一个字段
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public  object ReturnOracleResult(List<OracleParameter> parems, string Name);

        /// <summary>
        /// dynamic转jobject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  JObject dynamicToJObject(dynamic obj);
        /// <summary>
        /// 自定义dynamic转jobject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JObject dynamictoJobj(dynamic obj);
        /// <summary>
        /// string转T
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  T getStringToT<T>(string res);

        /// <summary>
        /// 查询sql返回datatable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  DataTable QuerySql(AbpvNextHISInterfaceDbContext db, string sql);

        /// <summary>
        /// 获得入参
        /// </summary>
        /// <returns></returns>
        public  OracleParameter GetInput(string name, object value);
        /// <summary>
        /// 获得入参
        /// </summary>
        /// <returns></returns>
        public  OracleParameter GetInput(string name, OracleDbType dbType, object value);
        /// <summary>
        /// 获得出参
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public  OracleParameter GetOutput(string name, OracleDbType dbType, int dataLength);
        /// <summary>
        /// 将dataset数据集转换成json对象
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ArrayList getJObject(DataSet ds);
        /// <summary>
        /// 将datatable数据集转换成json对象
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public  ArrayList getJObject(DataTable ds);
        /// <summary>
        /// 获取json返回得到的json数据
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetResult(List<OracleParameter> parems, DataSet ds, string ErrName = null);
        /// <summary>
        /// 分页返回
        /// </summary>
        /// <param name="parems">参数</param>
        /// <param name="ds">数据集</param>
        /// <param name="Page">第几行</param>
        /// <param name="PageNum">页行数</param>
        /// <param name="ErrName"></param>
        /// <returns></returns>
        public ObjectResult GetResult(List<OracleParameter> parems, DataSet ds, int Page, int PageNum, string ErrName = null);
        /// <summary>
        /// 获取json返回得到的json数据
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public  ObjectResult GetResultAndHaveSon(List<OracleParameter> parems, DataSet ds);

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public  string getSMSPost(string phone, string content);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<COM_ZHYY_CONFIG> GetZHYYConfigs();

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        /// <returns></returns>
        public  Task<COM_ZHYY_CONFIG> GetServiceConfigAsync(string ServiceName, string ServiceModule);
        /// <summary>
        /// 通过数据库进行绑定服务
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        public IActionResult BindService(AbpvNextHISInterfaceDbContext db, string ServiceName, string ServiceModule,JObject jobj);
        /// <summary>
        /// 绑定服务查询sql语句
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public IActionResult BindServiceForSql(AbpvNextHISInterfaceDbContext db, string ServiceName, string ServiceModule, JObject jobj);
        /// <summary>
        /// 费用查询进行分组
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetHosListGroup(DataSet ds);
        /// <summary>
        /// 获取微生物查询的分组集合
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public ObjectResult GetWswList(ArrayList arr);
    }
}
