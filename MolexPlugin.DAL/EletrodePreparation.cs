using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 备料
    /// </summary>
    public class EletrodePreparation
    {
        private List<int> length = new List<int>();
        private List<int> width = new List<int>();

        public EletrodePreparation()
        {
            this.length = GetContr("CuLength");
            this.width = GetContr("CuWidth");
            length.Sort();
            width.Sort();
        }
        /// <summary>
        /// 获取备料尺寸
        /// </summary>
        /// <param name="MaxOutline"></param>
        /// <returns></returns>
        public bool GetPreparation(ref int[] maxOutline)
        {
            bool isLength = false;
            bool isWidth = false;
            int[] pre = new int[2] { maxOutline[0], maxOutline[1] };

            if (maxOutline[0] > maxOutline[1])
            {
                isLength = IsOutline(length.ToArray(), ref pre[0]);
                isWidth = IsOutline(width.ToArray(), ref pre[1]);
            }
            else
            {
                isLength = IsOutline(width.ToArray(), ref pre[0]);
                isWidth = IsOutline(length.ToArray(), ref pre[1]);
            }
            if (!(isWidth && isLength))
            {
                pre[0] = ((int)(maxOutline[0] + 4) / 5) * 5;
                pre[1] = ((int)(maxOutline[1] + 4) / 5) * 5;
            }

            maxOutline[0] = pre[0];
            maxOutline[1] = pre[1];
            return isLength && isWidth;
        }

        private bool IsOutline(int[] wai, ref int max)
        {
            foreach (int k in wai)
            {
                if (max <= k)
                {
                    max = k;
                    return true;
                }
            }
            return false;
        }


        private List<int> GetContr(string controlType)
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
    }
}
