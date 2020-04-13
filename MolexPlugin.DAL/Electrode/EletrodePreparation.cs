using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    public class EletrodePreparation
    {
        private List<int> length = new List<int>();
        private List<int> width = new List<int>();

        public EletrodePreparation(string lengthName, string widthName)
        {
            this.length = GetContr(lengthName);
            this.width = GetContr(widthName);
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
        /// <summary>
        /// 获取标准料
        /// </summary>
        /// <param name="wai"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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
        private bool IsCriterion(int[] crit, int pre)
        {
            foreach (int k in crit)
            {
                if (pre == k)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 判断材料是否标准料
        /// </summary>
        /// <param name="pre"></param>
        /// <returns></returns>
        public bool IsPreCriterion(int[] pre)
        {
            if (pre[0] > pre[1])
            {
                return (IsCriterion(length.ToArray(), pre[0]) && (IsCriterion(width.ToArray(), pre[1])));
            }
            else
                return (IsCriterion(length.ToArray(), pre[1]) && (IsCriterion(width.ToArray(), pre[0])));
        }

        /// <summary>
        /// 数据库拿数据
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
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
