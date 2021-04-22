using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbpvNext.Application.HISInterface.IDBFactory
{
   public interface IFunService
    {
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken();
        /// <summary>
        /// 获取小程序图片
        /// </summary>
        /// <param name="ClinicNo"></param>
        /// <returns></returns>
        public Stream GetPictureurl(string ClinicNo);
        /// <summary>
        /// 下载Excel的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">数据</param>
        /// <param name="headers">表头</param>
        /// <returns></returns>
        public  string CreateExcelFromList(string JsonString, string Headerstring);
    }
}
