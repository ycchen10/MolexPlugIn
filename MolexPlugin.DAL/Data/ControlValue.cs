using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.DLL;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ControlValue
    {
        private static List<ControlEnum> controls = new List<ControlEnum>();
        public static List<ControlEnum> Controls
        {
            get
            {
                if (controls.Count == 0 || controls == null)
                {
                    ControlEnumNameDll dll = new ControlEnumNameDll();
                    return dll.GetList();
                }
                else
                {
                    return controls;
                }
            }
        }

        private ControlValue()
        {
          
        }
        /// <summary>
        /// 获取控件数字
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public List<int> GetContrForInt(string controlType)
        {
            List<int> control = new List<int>();
            var temp = ControlValue.Controls.GroupBy(a => a.ControlType);
            foreach (var i in temp)
            {
                if (i.Key == controlType)
                {
                    foreach (var k in i)
                    {
                        control.Add(Convert.ToInt32(k.EnumName));
                    }
                }
            }
            return control;
        }
        /// <summary>
        /// 获取控件字符串
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public List<string> GetContrForString(string controlType)
        {
            List<string> control = new List<string>();
            var temp = ControlValue.Controls.GroupBy(a => a.ControlType);
            foreach (var i in temp)
            {
                if (i.Key == controlType)
                {
                    foreach (var k in i)
                    {
                        control.Add(k.EnumName);
                    }
                }
            }
            return control;
        }

    }
}
