using AbpvNext.Application.HISInterface.IDBFactory;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbpvNext.Application.HISInterface.DBFactory
{
   public class LoggerService:ILoggerService
    {
        private  ILog logger;
        /// <summary>
        /// 配置log4net
        /// </summary>
        public LoggerService()
        {;
            this.logger = LogManager.GetLogger(AbpvNextApplicationModule.repository.Name, typeof(LoggerService));
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public  void Info(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Info(message);
            else
                logger.Info(message, exception);
        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public  void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Warn(message);
            else
                logger.Warn(message, exception);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public  void Error(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Error(message);
            else
                logger.Error(message, exception);
        }
    }
}
