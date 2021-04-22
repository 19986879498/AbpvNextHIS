using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNext.DDD.Domain.HISInterface.Entitys;
using AbpvNextEntityFrameworkCoreForOracle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AbpvNext.Application.HISInterface.DBFactory
{
   public class ZHYYDbMethods: IZHYYDbMethods
    {
        private readonly AbpvNextHISInterfaceDbContext dbContext;

        public ZHYYDbMethods(AbpvNextHISInterfaceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region 调用存储过程获取方法
        /// <summary>
        /// 调用存储过程获取方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="spName"></param>
        /// <param name="paramsters"></param>
        /// <returns></returns>
        public DataSet SqlQuery(AbpvNextHISInterfaceDbContext db, string spName, params OracleParameter[] paramsters)
        {
            OracleConnection connection = db.Database.GetDbConnection() as OracleConnection;
            ////OracleConnection connection = new OracleConnection(db.Database.GetDbConnection().ConnectionString);//db.Database.GetDbConnection() as OracleConnection
            //if (connection.State==ConnectionState.Closed)
            //{
            //    connection.Open();
            //}
            OracleDataAdapter adapter = null;
            DataSet set = null;
            using (OracleCommand command = new OracleCommand(spName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                command.Parameters.AddRange(paramsters);
                adapter = new OracleDataAdapter(command);

                set = new DataSet();
                adapter.Fill(set);
                adapter.SelectCommand.Parameters.Clear();
                adapter.Dispose();
                command.Parameters.Clear();
                command.Dispose();
                connection.Close();
                connection.Dispose();
                return set;
            }
        } 
        #endregion

        #region 获取查询
        /// <summary>
        /// 获取查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet SqlQuery(AbpvNextHISInterfaceDbContext db, string sql)
        {
            //创建Oracle连接对象
            OracleConnection conn = db.Database.GetDbConnection() as OracleConnection;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            //创建操作对象
            OracleCommand command = conn.CreateCommand();
            DataSet dataTable = new DataSet();
            command.CommandText = string.Format(sql);
            OracleDataAdapter oradata = new OracleDataAdapter();
            oradata.SelectCommand = command;
            oradata.Fill(dataTable);

            command.Parameters.Clear();
            conn.Close();
            int count = dataTable.Tables[0].Rows.Count;

            return dataTable;
        } 
        #endregion

        #region 查询返回一个字段
        /// <summary>
        /// 查询返回一个字段
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object ReturnOracleResult(List<OracleParameter> parems, string Name)
        {
            return parems.FirstOrDefault(u => u.ParameterName == Name).Value.ToString();
        } 
        #endregion

        #region dynamic转jobject
        /// <summary>
        /// dynamic转jobject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JObject dynamicToJObject(dynamic obj)
        {
            return JsonConvert.DeserializeObject<JObject>(obj.ToString());
        } 
        #endregion

        #region string转T
        /// <summary>
        /// 自定义dynamic转jobject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JObject dynamictoJobj(dynamic obj)
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(obj));
        } 
        #endregion

        #region string转T
        /// <summary>
        /// string转T
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T getStringToT<T>(string res)
        {
            return JsonConvert.DeserializeObject<T>(res);
        } 
        #endregion

        #region 查询sql返回datatable
        /// <summary>
        /// 查询sql返回datatable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable QuerySql(AbpvNextHISInterfaceDbContext db, string sql)
        {
            OracleConnection conn = db.Database.GetDbConnection() as OracleConnection; DataTable ds = new DataTable();
            try
            {
                conn.Open();
                OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                OracleCommand cmd = new OracleCommand(sql, conn);
                DataTable dt = new DataTable();
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 999;
                var reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                return dt;

                // ds.Load(reader);
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            return ds;
        } 
        #endregion

        #region 获得入参
        /// <summary>
        /// 获得入参
        /// </summary>
        /// <returns></returns>
        public OracleParameter GetInput(string name, object value)
        {
            var input = new OracleParameter(name, value);
            input.OracleDbType = OracleDbType.Varchar2;
            input.Direction = System.Data.ParameterDirection.Input;
            return input;
        } 
        #endregion


        #region 获得入参
        /// <summary>
        /// 获得入参
        /// </summary>
        /// <returns></returns>
        public OracleParameter GetInput(string name, OracleDbType dbType, object value)
        {
            var input = new OracleParameter(name, value);
            input.OracleDbType = dbType;
            input.Direction = System.Data.ParameterDirection.Input;
            return input;
        } 
        #endregion

        #region 获得出参
        /// <summary>
        /// 获得出参
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public OracleParameter GetOutput(string name, OracleDbType dbType, int dataLength)
        {
            var output = new OracleParameter(name, dbType, dataLength);
            output.Direction = System.Data.ParameterDirection.Output;
            return output;
        } 
        #endregion

        #region 将dataset数据集转换成json对象
        /// <summary>
        /// 将dataset数据集转换成json对象
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ArrayList getJObject(DataSet ds)
        {
            ArrayList arr = new ArrayList();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Dictionary<string, string> j = new Dictionary<string, string>();
                    for (int i = 0; i < item.ItemArray.Length; i++)
                    {
                        j.Add(ds.Tables[0].Columns[i].ColumnName.ToString(), item.ItemArray[i].ToString());
                    }
                    arr.Add(j);
                }
            }
            return arr;
        } 
        #endregion

        #region 将datatable数据集转换成json对象
        /// <summary>
        /// 将datatable数据集转换成json对象
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ArrayList getJObject(DataTable ds)
        {
            ArrayList arr = new ArrayList();
            if (ds == null)
            {
                return null;
            }
            if (ds.Rows.Count > 0)
            {
                foreach (DataRow item in ds.Rows)
                {
                    Dictionary<string, string> j = new Dictionary<string, string>();
                    for (int i = 0; i < item.ItemArray.Length; i++)
                    {
                        j.Add(ds.Columns[i].ColumnName.ToString(), item.ItemArray[i].ToString());
                    }
                    arr.Add(j);
                }
            }
            return arr;
        } 
        #endregion


        #region 获取json返回得到的json数据
        /// <summary>
        /// 获取json返回得到的json数据
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetResult(List<OracleParameter> parems, DataSet ds,string[] returnlist,string[] returntype)
        {
            var result = new ObjectResult("");
            if (returnlist.Length!=returntype.Length)
            {
               result= new ObjectResult(new { msg ="返回的类型与字段不匹配，请检查com_zhyy_config表", data = "查询无数据", code = 500 });
            }
           // ErrName = string.IsNullOrEmpty(ErrName) ? "ErrorMsg" : ErrName;
            int code = 0;
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            //循环数据库中的返回值
            for (int i = 0; i < returnlist.Length; i++)
            {
                if (int.Parse(returntype[i])== 121)
                {
                    continue;
                }
                string Name = returnlist[i];
                object val = GetParameterByType((OracleDbType)int.Parse(returntype[i]), parems.Where(i => i.ParameterName == Name).FirstOrDefault().Value.ToString());
                if (Name.Contains("Code"))
                {
                    val = (int)val == 1 ? 200 : 500;
                    Name = "code";
                }
                else if (Name.Contains("Msg"))
                {
                    Name = "msg";
                }
                pairs.Add(Name, val);
               
            }
            var data = getJObject(ds);
            pairs.Add("data", data);
            result = new ObjectResult(pairs);
          

        //Error:
        //     result = new { msg = parems.Where(i => i.ParameterName == ErrName).FirstOrDefault().Value.ToString(), data = "查询无数据", code = 404 };
            return result;
        }
        #endregion

        #region 获取json返回得到的json数据
        /// <summary>
        /// 获取json返回得到的json数据
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetResult(List<OracleParameter> parems, DataSet ds, string ErrName = null)
        {
            ErrName = string.IsNullOrEmpty(ErrName) ? "ErrorMsg" : ErrName;
            int code = 0;
            try
            {
                code = Convert.ToInt32(parems.Where(i => i.ParameterName == "ReturnCode").FirstOrDefault().Value.ToString());
            }
            catch (Exception)
            {
                code = 404;
            }
            var data = getJObject(ds);
            IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(data));
            string jsonstrData = string.Empty;
            if (data != null && data.Count > 0)
            {
                jsonstrData = JsonConvert.SerializeObject(data);
            }
            else
            {
                goto Error;
            }
            if (code == 1)
            {
                string Msg = parems.Where(i => i.ParameterName == ErrName).FirstOrDefault().Value.ToString();
                var res = new { msg = Msg, data = data, code = 200 };
                return new ObjectResult(res);

            }
            else
            {
                goto Error;

            }

        Error:
            var result = new { msg = parems.Where(i => i.ParameterName == ErrName).FirstOrDefault().Value.ToString(), data = "查询无数据", code = 404 };
            return new ObjectResult(result);
        }
        #endregion

        #region 分页返回
        /// <summary>
        /// 分页返回
        /// </summary>
        /// <param name="parems">参数</param>
        /// <param name="ds">数据集</param>
        /// <param name="Page">第几行</param>
        /// <param name="PageNum">页行数</param>
        /// <param name="ErrName"></param>
        /// <returns></returns>
        public ObjectResult GetResult(List<OracleParameter> parems, DataSet ds, int Page, int PageNum, string ErrName = null)
        {
            ErrName = string.IsNullOrEmpty(ErrName) ? "ErrorMsg" : ErrName;
            int code = 0;
            try
            {
                code = Convert.ToInt32(parems.Where(i => i.ParameterName == "ReturnCode").FirstOrDefault().Value.ToString());
            }
            catch (Exception)
            {
                code = 404;
            }
            var data = getJObject(ds);
            string jsonstrData = string.Empty;
            if (data != null && data.Count > 0)
            {
                jsonstrData = JsonConvert.SerializeObject(data);
            }
            else
            {
                goto Error;
            }
            //分页 
            IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(data));
            j = j.Skip((Page - 1) * PageNum).Take(PageNum).ToList();

            if (code == 1)
            {
                string Msg = parems.Where(i => i.ParameterName == ErrName).FirstOrDefault().Value.ToString();
                var res = new { msg = Msg, data = j, code = 200, count = data.Count };
                return new ObjectResult(res);

            }
            else
            {
                goto Error;

            }

        Error:
            var result = new { msg = parems.Where(i => i.ParameterName == ErrName).FirstOrDefault().Value.ToString(), data = "查询无数据", code = 404 };
            return new ObjectResult(result);
        } 
        #endregion

        #region 获取json返回得到的json数据
        /// <summary>
        /// 获取json返回得到的json数据
        /// </summary>
        /// <param name="parems"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetResultAndHaveSon(List<OracleParameter> parems, DataSet ds)
        {
            int code = Convert.ToInt32(parems.Where(i => i.ParameterName == "ReturnCode").FirstOrDefault().Value.ToString());
            var data = getJObject(ds);
            string jsonstrData = string.Empty;
            if (data != null && data.Count > 0)
            {
                jsonstrData = JsonConvert.SerializeObject(data);
            }
            else
            {
                goto Error;
            }
            if (code == 1)
            {
                IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(data));
                string Msg = parems.Where(i => i.ParameterName == "ErrorMsg").FirstOrDefault().Value.ToString();
                string[] ids = j.Select(s => s.GetValue("subjectId").ToString()).Distinct().ToArray();
                List<object> objects = new List<object>();
                foreach (string item in ids)
                {//, subjectName=j.FirstOrDefault(u=>u.GetValue("subjectId").ToString()==item).GetValue("subjectName").ToString() 
                    objects.Add(new JsonResult(new { subjectId = item, subjectName = j.FirstOrDefault(u => u.GetValue("subjectId").ToString() == item).GetValue("subjectName").ToString(), details = (from JObject x in j where x.GetValue("subjectId").ToString() == item select x).Select(s => new { deptName = s.GetValue("deptName").ToString(), deptId = s.GetValue("DeptId").ToString() }) }).Value);
                }
                var res = new { msg = Msg, data = objects, code = 200 };
                return new ObjectResult(res);

            }
            else
            {
                goto Error;

            }

        Error:
            var result = new { msg = parems.Where(i => i.ParameterName == "ErrorMsg").FirstOrDefault().Value.ToString(), data = new ArrayList(), code = 404 };
            return new ObjectResult(result);
        } 
        #endregion


        #region 短信接口
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public  string getSMSPost(string phone, string content)
        {
            string data = $"<request><id>1</id><phone>{phone}</phone><content>{content}</content></request>";
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://192.168.1.110:8081/SmsServer.ashx");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            byte[] encodedBytes = Encoding.UTF8.GetBytes(data);
            myHttpWebRequest.ContentLength = encodedBytes.Length;

            // Write encoded data into request stream
            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(encodedBytes, 0, encodedBytes.Length);
            requestStream.Close();
            HttpWebResponse result;
            try
            {
                result = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return string.Empty;
            }

            if (result.StatusCode == HttpStatusCode.OK)
            {
                Stream mystream = result.GetResponseStream();
                StreamReader reader = new StreamReader(mystream);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader.ReadToEnd());
                string res = JsonConvert.SerializeXmlNode(doc);
                return res;
            }

            return "";

        }
        #endregion

        #region 获取查询实体
        /// <summary>
        /// 获取查询实体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<COM_ZHYY_CONFIG> GetZHYYConfigs()
        {
            string name = dbContext.Database.GetDbConnection().ConnectionString;
            return this.dbContext.COM_ZHYY_CONFIG;
        } 
        #endregion

        #region 获取实体
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        /// <returns></returns>
        public async Task<COM_ZHYY_CONFIG> GetServiceConfigAsync(string ServiceName, string ServiceModule)
        {
            return await this.dbContext.COM_ZHYY_CONFIG.FirstOrDefaultAsync(u => u.SERVICENAME == ServiceName && u.SERVICEMODULE == ServiceModule);
        } 
        #endregion

        #region 绑定数据库中配置的服务
        /// <summary>
        /// 绑定数据库中配置的服务
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        public IActionResult BindService(AbpvNextHISInterfaceDbContext db, string ServiceName, string ServiceModule, JObject jobj)
        {
            COM_ZHYY_CONFIG myconfig = this.GetServiceConfigAsync(ServiceName, ServiceModule).Result;
            if (myconfig == null)
            {
                return new JsonResult(new { msg = "请求失败", data = $"服务名为{ServiceModule}/{ServiceName}未配置不可使用！", code = "500" });
            }
            if (myconfig.VALID_FALG == "0")
            {
                return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}该服务的已被作废！", code = "500" });
            }
            string[] inparameters = myconfig.INPARAMETERNAMES == null ? new string[0] : myconfig.INPARAMETERNAMES.Split('|');
            string[] intypes = myconfig.INPARAMETERTYPES == null ? new string[0] : myconfig.INPARAMETERTYPES.Split('|');
            string[] procedureprops = myconfig.INPROCEDUREPROP == null ? new string[0] : myconfig.INPROCEDUREPROP.Split('|');
            string[] outparameters = myconfig.OUTPARAMETERNAMES == null ? new string[0] : myconfig.OUTPARAMETERNAMES.Split('|');
            string[] outtypes = myconfig.OUTPARAMETERTYPES == null ? new string[0] : myconfig.OUTPARAMETERTYPES.Split('|');
            if (inparameters.Length != intypes.Length)
            {
                return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}入参和入参的类型无法对应，请检查com_zhyy_config表中的配置", code = "500" });
            }
            else if (inparameters.Length != procedureprops.Length)
            {
                return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}入参和数据库入参无法对应，请检查com_zhyy_config表中的配置", code = "500" });
            }
            else if (outparameters.Length != outtypes.Length)
            {
                return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}出参和出参类型无法对应，请检查com_zhyy_config表中的配置", code = "500" });
            }
            //输入参数
            List<OracleParameter> parems = new List<OracleParameter>();
            try
            {
                for (int i = 0; i < inparameters.Length; i++)
                {
                    parems.Add(GetInput(procedureprops[i], (OracleDbType)Int32.Parse(intypes[i]) , GetParameterByType((OracleDbType)Int32.Parse(intypes[i]), jobj.GetValue(inparameters[i]).ToString())));
                }

            }
            catch (Exception)
            {
                return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}请检查您所传递的参数是否有误！", code = "500" });
            }
            //输出参数(绑定数据库中的配置表)
            for (int i = 0; i < outparameters.Length; i++)
            {
                int len = Int32.Parse(outtypes[i]) == 121 ? 1024 : (Int32.Parse(outtypes[i]) == 112 ? 200 : 200);
                parems.Add(GetOutput(outparameters[i], (OracleDbType)Int32.Parse(outtypes[i]), len));
            }
            //parems.Add(GetOutput("ResultSet", (OracleDbType)121, 1024));
            //parems.Add(GetOutput("ReturnCode", (OracleDbType)112, 200));
            //parems.Add(GetOutput("ErrorMsg", (OracleDbType)126, 200));
            db = db == null ? dbContext : db;
            var ds = SqlQuery(db, myconfig.PROCEDURENAME, parems.ToArray());
            ObjectResult obj = GetResult(parems, ds, outparameters, outtypes);
            if (myconfig.ISFY == "1")
            {
                if (!jobj.ContainsKey("Page"))
                {
                    return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}此接口需要分页，没有找到参数名为Page的参数！", code = "500" });
                }
                if (!jobj.ContainsKey("pageSize"))
                {
                    return new JsonResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}此接口需要分页，没有找到参数名为pageSize的参数！", code = "500" });
                }
                int page = int.Parse(jobj.GetValue("Page", StringComparison.OrdinalIgnoreCase).ToString());
                int pageCount = int.Parse(jobj.GetValue("pageSize", StringComparison.OrdinalIgnoreCase).ToString());
                obj = GetResult(parems, ds, page, pageCount);
            }
            if (Convert.ToInt32(myconfig.ISSONSHOW) == (int)SonShowType.科室查询)
            {
                obj = GetResultAndHaveSon(parems, ds);
            }
            else if (Convert.ToInt32(myconfig.ISSONSHOW) == (int)SonShowType.微生物查询)
            {

            }
            else if (Convert.ToInt32(myconfig.ISSONSHOW) == (int)SonShowType.费用查询)
            {
                if (!jobj.ContainsKey("type"))
                {
                    obj = new ObjectResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}没有找到参数名为type的参数！", code = "500" });
                }
                if (jobj.GetValue("type").ToString() == "2")
                {
                    obj = this.GetHosListGroup(ds);
                }else if (jobj.GetValue("type").ToString() == "3")
                {
                    obj = this.GetHosListGroup(ds);
                }
                else
                {
                    //Console.WriteLine("返回参数：\n" + JsonConvert.SerializeObject(Methods.GetResult(parems, ds)));
                    obj = GetResult(parems, ds, outparameters,outtypes);
                }
            }
            return obj;
        }
        #endregion


        #region 根据oracle的类型进行改变对应的类型
        /// <summary>
        /// 根据oracle的类型进行改变对应的类型
        /// </summary>
        /// <param name="db"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetParameterByType(OracleDbType db, string value)
        {
            switch (db)
            {
                case OracleDbType.BFile:
                    return value;
                case OracleDbType.Blob:
                    return value;
                case OracleDbType.Byte:
                    return value;
                case OracleDbType.Char:
                    return value;
                case OracleDbType.Clob:
                    return value;
                case OracleDbType.Date:
                    return DateTime.Parse(value);
                case OracleDbType.Decimal:
                    return decimal.Parse(value);
                case OracleDbType.Double:
                    return double.Parse(value);
                case OracleDbType.Long:
                    return long.Parse(value);
                case OracleDbType.LongRaw:
                    return long.Parse(value);
                case OracleDbType.Int16:
                    return Int16.Parse(value);
                case OracleDbType.Int32:
                    return Int32.Parse(value);
                case OracleDbType.Int64:
                    return Int64.Parse(value);
                case OracleDbType.IntervalDS:
                    return value;
                case OracleDbType.IntervalYM:
                    return value;
                case OracleDbType.NClob:
                    return value;
                case OracleDbType.NChar:
                    return value;
                case OracleDbType.NVarchar2:
                    return value;
                case OracleDbType.Raw:
                    return value;
                case OracleDbType.RefCursor:
                    return value;
                case OracleDbType.Single:
                    return value;
                case OracleDbType.TimeStamp:
                    return value;
                case OracleDbType.TimeStampLTZ:
                    return value;
                case OracleDbType.TimeStampTZ:
                    return value;
                case OracleDbType.Varchar2:
                    return value;
                case OracleDbType.XmlType:
                    return value;
                case OracleDbType.BinaryDouble:
                    return value;
                case OracleDbType.BinaryFloat:
                    return value;
                case OracleDbType.Boolean:
                    return Boolean.Parse(value);
                default:
                    return value;
            }
        } 
        #endregion

        #region 绑定查询sql的服务
        /// <summary>
        /// 绑定查询sql的服务
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ServiceModule"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public IActionResult BindServiceForSql(AbpvNextHISInterfaceDbContext db, string ServiceName, string ServiceModule, JObject jobj)
        {
            COM_ZHYY_CONFIG myconfig = this.GetServiceConfigAsync(ServiceName, ServiceModule).Result;
            if (myconfig == null)
            {
                return new ObjectResult(new { msg = "请求失败", data = $"服务名为{ServiceModule}/{ServiceName}未配置不可使用！", code = "500" });
            }
            if (myconfig.VALID_FALG == "0")
            {
                return new ObjectResult(new { msg = "请求失败", data = $"{ServiceModule}/{ServiceName}该服务的已被作废！", code = "500" });
            }
            string[] inparameters = myconfig.INPARAMETERNAMES == null ? new string[0] : myconfig.INPARAMETERNAMES.Split('|');
            string[] intypes = myconfig.INPARAMETERTYPES == null ? new string[0] : myconfig.INPARAMETERTYPES.Split('|');
            string[] procedureprops = myconfig.INPROCEDUREPROP == null ? new string[0] : myconfig.INPROCEDUREPROP.Split('|');
            string[] outparameters = myconfig.OUTPARAMETERNAMES == null ? new string[0] : myconfig.OUTPARAMETERNAMES.Split('|');
            string[] outtypes = myconfig.OUTPARAMETERTYPES == null ? new string[0] : myconfig.OUTPARAMETERTYPES.Split('|');
            List<object> objlist = new List<object>();
            for (int i = 0; i < inparameters.Length; i++)
            {
                if (myconfig.SERVICENAME == "QueryZBInfoByDate" && inparameters[i] == "date")
                {
                    objlist.Add(Convert.ToDateTime(jobj.GetValue(inparameters[i]).ToString()).ToString("yyyy-MM-01"));
                }
                else
                {
                    objlist.Add(jobj.GetValue(inparameters[i]).ToString());
                }

            }
            ObjectResult res = new ObjectResult("");
            string Sql = string.Format(myconfig.PROCEDURENAME, objlist.ToArray());
            db = db == null ? dbContext : db;
            var dt = this.QuerySql(db, Sql);
            ArrayList arr = this.getJObject(dt);
            if (Convert.ToInt32(myconfig.ISFY) == (int)SonShowType.微生物查询)
            {
                res = GetWswList(arr);
            }
            else
            {
                res= new ObjectResult(new { msg = $"查询成功！", data = arr, code = 200 });
            }
            if (arr == null)
            {
                res = new ObjectResult(new { msg = $"{ServiceModule}/{ServiceName}没有找到如何数据的信息！", data = "查询结果为空", code = 404 });
            }
            if (arr.Count == 0)
            {
                res = new ObjectResult(new { msg = $"{ServiceModule}/{ServiceName}没有找到如何数据的信息！", data = "查询结果为空", code = 404 });
            }
            return res;
        }
        #endregion

        #region 费用分组查询
        /// <summary>
        /// 费用分组查询
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ObjectResult GetHosListGroup(DataSet ds)
        {
            ArrayList arr = this.getJObject(ds);
            if (arr.Count == 0 || arr == null)
            {
                return new ObjectResult(new { msg = "没有找到患者详细的信息！", data = "查询结果为空", code = 404 });
            }
            IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(arr));
            string[] ids = j.Select(s => s.GetValue("hospitalNo").ToString()).Distinct().ToArray();
            List<object> objects = new List<object>();
            foreach (string item in ids)
            {
                objects.Add(new JsonResult(new { hospitalName = j.FirstOrDefault(u => u.GetValue("hospitalNo").ToString() == item).GetValue("hospitalName").ToString(), patientName = j.FirstOrDefault(u => u.GetValue("hospitalNo").ToString() == item).GetValue("patientName").ToString(), hospitalNo = item.ToString(), inTime = j.FirstOrDefault(u => u.GetValue("hospitalNo").ToString() == item).GetValue("inTime").ToString(), outTime = j.FirstOrDefault(u => u.GetValue("hospitalNo").ToString() == item).GetValue("outTime").ToString(), detail = (from JObject x in j where x.GetValue("hospitalNo").ToString() == item select x).Select(s => new { titleName = s.GetValue("titleName").ToString(), unit = s.GetValue("unit").ToString(), num = s.GetValue("num").ToString(), price = s.GetValue("price").ToString() }) }).Value);
            }
            //  Console.WriteLine("返回参数：\n" + JsonConvert.SerializeObject(new JsonResult(new { msg = "查询成功！", data = objects, code = 200 })));
            return new ObjectResult(new { msg = "查询成功！", data = objects, code = 200 });
        }
        #endregion

        #region 查询微生物分组
        /// <summary>
        /// 查询微生物分组
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public ObjectResult GetWswList(ArrayList arr)
        {
            IEnumerable<JObject> j = JsonConvert.DeserializeObject<IEnumerable<JObject>>(JsonConvert.SerializeObject(arr));
            string[] ids = j.Select(s => s.GetValue("itemName").ToString()).Distinct().ToArray();
            List<object> objects = new List<object>();
            foreach (string item in ids)
            {
                objects.Add(new JsonResult(new { item = j.FirstOrDefault(u => u.GetValue("itemName").ToString() == item).GetValue("item").ToString(), itemName = item.ToString(), details = (from JObject x in j where x.GetValue("itemName").ToString() == item select x).Select(s => new { name = s.GetValue("name").ToString(), value = s.GetValue("value").ToString(), reference = s.GetValue("reference").ToString(), unit = s.GetValue("unit").ToString(), status = s.GetValue("status").ToString(), remark = s.GetValue("remark").ToString() }) }).Value);
            }
            return new ObjectResult(new { msg = "查询成功！", data = objects, code = 200 });
        } 
        #endregion

        public enum SonShowType
        {
            科室查询=1,
            微生物查询=2,
            费用查询=3
        }
    }
}
