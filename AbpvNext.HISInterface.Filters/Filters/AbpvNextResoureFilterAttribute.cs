using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbpvNext.HISInterface.Filters.Filters
{
    public class AbpvNextResoureFilterAttribute : Attribute, IResourceFilter, IFilterMetadata, IOrderedFilter
    {
        private static Dictionary<string, IActionResult> myServiceCache = new Dictionary<string, IActionResult>();
        public int Order => 0;
        /// <summary>
        /// 使用ResourceFilter 实现缓存效果
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string key = context.HttpContext.Request.Path;
            var result = (ObjectResult)context.Result;
            Console.WriteLine("请求结果："+JsonConvert.SerializeObject(result.Value));
            if (myServiceCache.ContainsKey(key))
            {
                //拦截器
                context.Result = myServiceCache[key];
            }
        }
        /// <summary>
        ///  使用ResourceFilter 实现缓存效果
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string key = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            var RequestData = context.HttpContext.Request.Body;
            //获取请求的Body
            //数据流倒带 context.HttpContext.Request.EnableRewind();
            //string RequestData = string.Empty;
            //if (context.HttpContext.Request.Body.CanRead)
            //{
            //    using (var requestSm = context.HttpContext.Request.Body)
            //    {
            //        //requestSm.Position = 0;
            //        var reader = new StreamReader(requestSm, Encoding.UTF8);
            //        RequestData = reader.ReadToEnd();
            //    }
            //}
            //Console.WriteLine(RequestData);
            //if (context.Result!=null)
            //{
            //    if (!string.IsNullOrEmpty(key))
            //    {
            //        myServiceCache.Add(key, context.Result);
            //    } 
            //}
        }
    }
}
