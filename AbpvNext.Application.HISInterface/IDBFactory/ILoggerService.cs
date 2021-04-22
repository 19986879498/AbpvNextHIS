using System;
using System.Collections.Generic;
using System.Text;

namespace AbpvNext.Application.HISInterface.IDBFactory
{
   public interface ILoggerService
    {
        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(string message, Exception exception = null);
        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warn(string message, Exception exception = null);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception = null);
    }
}
