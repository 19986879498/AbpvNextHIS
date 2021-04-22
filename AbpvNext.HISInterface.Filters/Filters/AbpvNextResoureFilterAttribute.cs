using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
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
            if (context.Result!=null)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    myServiceCache.Add(key, context.Result);
                } 
            }
        }
    }
}
