using System;
using System.Threading;

namespace Tools.App_Code
{
    public class LogHelp
    {
        /// <summary>  
        /// 日志锁定  
        /// </summary>  
        private readonly static Object Lok = new Object();

        /// <summary>  
        /// 记录日志  
        /// </summary>  
        public static void Log(string txt)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                if (string.IsNullOrWhiteSpace(txt)) return;

                try
                {
                    lock (Lok)
                    {
                        string logDir = AppDomain.CurrentDomain.BaseDirectory;
                        string logPath = string.Format(@"{0}\log", logDir);
                        string logFile = string.Format(@"{0}\{1}.log", logPath, DateTime.Now.ToString(@"yy-MM-dd"));
                        //  
                        string logContent = string.Format("{0}\t{1}\r\n", DateTime.Now.ToString(@"HH:mm:ss"), txt);
                        System.IO.Directory.CreateDirectory(logPath);
                        System.IO.File.AppendAllText(logFile, logContent);
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            });
        }
    }
}