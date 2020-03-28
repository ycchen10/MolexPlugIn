using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.DLL;
using MolexPlugin.Model;
using System.Runtime.Serialization.Formatters.Binary;

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
                    //ControlEnumNameDll dll = new ControlEnumNameDll();
                    //return dll.GetList();
                    return Deserialize();
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

        /// <summary>
        /// 序列化
        /// </summary>
        public static void Serialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string contrPath = dllPath.Replace("application\\", "Cofigure\\SerializeContr.dat");
            if (File.Exists(contrPath))
                File.Delete(contrPath);
            List<ControlEnum> users = new ControlEnumNameDll().GetList();
            FileStream fs = new FileStream(contrPath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, users);
            fs.Close();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public static List<ControlEnum> Deserialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string contrPath = dllPath.Replace("application\\", "Cofigure\\SerializeContr.dat");
            if (File.Exists(contrPath))
            {
                FileStream fs = new FileStream(contrPath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(fs) as List<ControlEnum>;
            }
            return null;
        }
    }
}
