using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HISInterface.Logic
{
   public class Functions
    {
        /// <summary>
        /// ��ȡaccess_token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            string token = string.Empty;
            try
            {
                //΢��С����ӿ�
                string appID = "wx2335bbc9cdff2768";
                string appSecret = "a4971ccff81e9c31d13e944602d7e0c1";
                //��ȡ΢��token
                string token_url = "https://api.weixin.qq.com/cgi-bin/token?appid=" + appID + "&secret=" + appSecret + "&grant_type=client_credential";
               // Log.Logger.GetLog("Url:" + token_url);
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(token_url);
                //����ʽ
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
        //��ȡС����ͼƬ
        public static Stream GetPictureurl(string ClinicNo)
        {
            try
            {
                string token = Functions.GetAccessToken();
                if (string.IsNullOrEmpty(token))
                    return null;
                string jsonParam = "{\"page\":\"pages/payment_record_sao/index\",\"width\":280,\"scene\":\"hosid=1&clino=" + ClinicNo + "\"}";

                string strURL = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + token;
                //����һ��HTTP����  
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
                //Post����ʽ  
                request.Method = "POST";
                //��������
                request.ContentType = "application/json;charset=utf-8";

                //���ò�����������URL���� 

                string paraUrlCoded = jsonParam;//System.Web.HttpUtility.UrlEncode(jsonParas);   

                byte[] payload;
                //��Json�ַ���ת��Ϊ�ֽ�  
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                //���������ContentLength   
                request.ContentLength = payload.Length;
                //�������󣬻�������� 

                Stream writer;
                try
                {
                    writer = request.GetRequestStream();//��ȡ����д���������ݵ�Stream����
                }
                catch (Exception)
                {
                    writer = null;
                    Console.Write("���ӷ�����ʧ��!");
                }
                //���������д����
                writer.Write(payload, 0, payload.Length);
                writer.Close();//�ر�������
                // String strValue = "";//strValueΪhttp��Ӧ�����ص��ַ���
                HttpWebResponse response;
                try
                {
                    //�����Ӧ��
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
        /// ����Excel�ķ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">����</param>
        /// <param name="headers">��ͷ</param>
        /// <returns></returns>
        public static string CreateExcelFromList(string JsonString, string Headerstring)
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
                //����sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                // worksheet.Cells.LoadFromCollection(dataList, true);
                //��ͷ�ֶ�
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
        public class RecordResult
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }
    }
}