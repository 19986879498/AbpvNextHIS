using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbpvNext.HISInterface.Filters.Filters
{
   public class AbpvNextExceptionFilterAttribute:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                Console.WriteLine($"程序出现异常：{context.Exception.Message}");
                context.Result = new ObjectResult(new { code = 500, Msg = "出现异常" + context.Exception.Message });
                context.ExceptionHandled = true;
            }
            base.OnException(context);
        }
    }
}
