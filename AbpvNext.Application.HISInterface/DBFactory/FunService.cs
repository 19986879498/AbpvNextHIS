﻿using AbpvNext.Application.HISInterface.IDBFactory;
using AbpvNext.DDD.Domain.HISInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AbpvNext.Application.HISInterface.DBFactory
{
    public class FunService: IFunService
    {
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public  string GetAccessToken()
        {
            string token = string.Empty;
            try
            {
                //微信小程序接口
                string appID = "wx2335bbc9cdff2768";
                string appSecret = "a4971ccff81e9c31d13e944602d7e0c1";
                //获取微信token
                string token_url = "https://api.weixin.qq.com/cgi-bin/token?appid=" + appID + "&secret=" + appSecret + "&grant_type=client_credential";
                // Log.Logger.GetLog("Url:" + token_url);
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(token_url);
                //请求方式
                myRequest.Method = "GET";
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();
                // Log.Logger.GetLog("Content:" + content.ToString());
                myResponse.Close();
                reader.Dispose();
                //var result= JsonConvert.DeserializeObject(content);
                var result = JsonConvert.DeserializeObject<RecordResult>(content);
                token = result.access_token.ToString();
            }
            catch (Exception ex)
            {
                token = "";

            }
            // Log.Logger.GetLog("Token:" + token.ToString());
            return token;
        }
        /// <summary>
        /// 获取小程序图片
        /// </summary>
        /// <param name="ClinicNo"></param>
        /// <returns></returns>
        public Stream GetPictureurl(string ClinicNo)
        {
            try
            {
                string token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                    return null;
                string jsonParam = "{\"page\":\"pages/payment_record_sao/index\",\"width\":280,\"scene\":\"hosid=1&clino=" + ClinicNo + "\"}";

                string strURL = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + token;
                //创建一个HTTP请求  
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
                //Post请求方式  
                request.Method = "POST";
                //内容类型
                request.ContentType = "application/json;charset=utf-8";

                //设置参数，并进行URL编码 

                string paraUrlCoded = jsonParam;//System.Web.HttpUtility.UrlEncode(jsonParas);   

                byte[] payload;
                //将Json字符串转化为字节  
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                //设置请求的ContentLength   
                request.ContentLength = payload.Length;
                //发送请求，获得请求流 

                Stream writer;
                try
                {
                    writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
                }
                catch (Exception)
                {
                    writer = null;
                    Console.Write("连接服务器失败!");
                }
                //将请求参数写入流
                writer.Write(payload, 0, payload.Length);
                writer.Close();//关闭请求流
                // String strValue = "";//strValue为http响应所返回的字符流
                HttpWebResponse response;
                try
                {
                    //获得响应流
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                Stream s = response.GetResponseStream();
                return s;
            }

            catch (Exception)
            {

                throw;
            }
            return null;
        }
        /// <summary>
        /// 下载Excel的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">数据</param>
        /// <param name="headers">表头</param>
        /// <returns></returns>
        public  string CreateExcelFromList(string JsonString, string Headerstring)
        {
            List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(JsonString);
            List<string> headers = Headerstring.Split(',').ToList();
            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }
            string sFileName = $@"tempExcel_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            string[] excelArr = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                // worksheet.Cells.LoadFromCollection(dataList, true);
                //表头字段
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    for (int j = 0; j < jlist.Count; j++)
                    {
                        JObject itemjob = jlist[j];
                        if (!itemjob.ContainsKey(headers[i]))
                        {
                            worksheet.Cells[excelArr[i] + (2 + j)].Value = "";
                        }
                        worksheet.Cells[excelArr[i] + (2 + j)].Value = itemjob.TryGetValue(headers[i], StringComparison.OrdinalIgnoreCase, out JToken val) == true ? val.ToString() : "";
                    }
                }

                package.Save();
            }
            return path;
        }
    }
}
