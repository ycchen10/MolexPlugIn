using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Basic
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LogMgr
    {
        //<summary>   
        //保存日志的文件夹   
        //<summary>   
        // private static string logPath = Utils.GetWorkDir() + @"logfolder\";
        private static string logPath = "C:\\temp\\" + @"logfolder\";
        // private static string logPath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Substring(0,System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Length-14) + @"logfolder\";

        //<summary>   
        //写日志   
        //<summary>   
        public static void WriteLog(string msg)
        {
            try
            {
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }

                System.IO.StreamWriter sw = System.IO.File.AppendText(
                       logPath + " " +
                       DateTime.Now.ToString("yyyyMMdd") + "Error.Log"
                   );
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:  ") + msg);
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("在LogManager类中操作WriteLog方法时异常：" + ex.Message);
            }
        }
    }
}
