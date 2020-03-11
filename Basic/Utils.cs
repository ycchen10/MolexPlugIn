using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.BlockStyler;

namespace Basic
{
    public class Utils
    {
        private static string m_workDir = string.Empty;

        /// <summary> 
        /// 获取工程路径 
        /// </summary> 
        /// <returns></returns> 
        /// 
        public static string GetWorkDir()
        {
            if (m_workDir.Equals(string.Empty))
            {
                string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                
                if (_CodeBase.Contains("C:") || _CodeBase.Contains("D:") || _CodeBase.Contains("F:") || _CodeBase.Contains("E:"))
                    _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8); 
                else
                    _CodeBase = _CodeBase.Substring(5, _CodeBase.Length - 8);

                string[] arrSection = _CodeBase.Split(new char[] { '/' });

                string _FolderPath = "";
                for (int i = 0; i < arrSection.Length - 1; i++)
                {
                    _FolderPath += arrSection[i] + "/";
                }

                m_workDir = _FolderPath.Substring(0, _FolderPath.Length - 12);
            }

            return m_workDir;
        }
    }
}
