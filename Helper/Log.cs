using System;
using System.IO;
using log4net;

namespace Helper
{
    public static class Log
    {
        //private static readonly ILog _log =
        //    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private static readonly ILog _log = LogManager.GetLogger(" ");
        private static readonly ILog _log = LogManager.GetLogger("MainLog");

        public static void Debug(object message, Exception exception = null)
        {
           _log.Debug(message, exception);
        }
        public static void Info(object message, Exception exception = null)
        {
            _log.Info(message, exception);
        }
        public static void Warn(object message, Exception exception = null)
        {
            _log.Warn(message, exception);
        }

        public static void Error(object message, Exception exception = null)
        {
            _log.Error(message, exception);
        }

        public static event Action<LogLevel, string> LogEvent;

        public static void Run(LogLevel level, string msg,Exception ex = null)
        {
            LogEvent?.Invoke(level, msg);
            switch (level)
            {
                case LogLevel.Info:
                    Info(msg, ex);
                    break;
                case LogLevel.Warming:
                    Warn(msg, ex);
                    break;
                case LogLevel.Error:
                    Error(msg, ex);
                    break;
                default: return;
            }
        }

        public static void DeleteFile(int saveDay, string fileDirect = @".\Logs",string fileSuffix = "*")
        {
            DateTime nowTime = DateTime.Now;

            string[] files = Directory.GetFiles(fileDirect, fileSuffix, SearchOption.AllDirectories);  //获取该目录下所有文件
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                TimeSpan t = nowTime - fileInfo.LastWriteTime;  
                int day = t.Days;
                if (day > saveDay)   //保存的时间 ；  单位：天
                {
                    System.IO.File.Delete(file);  //删除超过时间的文件
                }
            }

        }
    }
    public static class LogConsole
    {
        //private static readonly ILog _log =
        //    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private static readonly ILog _log = LogManager.GetLogger(" ");
        private static readonly ILog _log = LogManager.GetLogger("logerror");

        public static void Debug(object message, Exception exception = null)
        {
            _log.Debug(message, exception);
        }
        public static void Info(object message, Exception exception = null)
        {
            _log.Info(message, exception);
        }
        public static void Warn(object message, Exception exception = null)
        {
            _log.Warn(message, exception);
        }

        public static void Error(object message, Exception exception = null)
        {
            _log.Error(message, exception);
        }
    }

    public enum LogLevel
    {
        Info,
        Warming,
        Error
    }
}
