using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Basic
{
    public class ClassItem 
    {
        public static Session theSession ;
        public static UI theUi ;
        public static UFSession theUFSession ;
        private static ListingWindow lw;
        private static LogFile lf;
        private static NXMessageBox mb;
        static ClassItem()
        {
            theSession = Session.GetSession();
            theUi = UI.GetUI();
            theUFSession = UFSession.GetUFSession();
            
            lw = theSession.ListingWindow;
            lf = theSession.LogFile;
            mb = theUi.NXMessageBox;

        }
       
        public static int MessageBox(string msg, NXMessageBox.DialogType type)
        {
            return mb.Show("错误", type, msg);
        }

        /// <summary>
        /// 输出到信息窗口
        /// </summary>
        /// <param name="msg"></param>
        public static void Print(string msg)
        {
            if (!lw.IsOpen)
            {
                lw.Open();
            }
            lw.WriteLine(msg);
        }
        /// <summary>
        /// 写入日志文件
        /// </summary>
        /// <param name="str"></param>
        public static void WriteLogFile(string str)
        {
            lf.WriteLine(str);
        }
    }
}
