using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 计算刀具
    /// </summary>
    public class ComputeTool
    {

        private double width;
        public double[] twice = new double[] { 4, 3, 2, 1.5, 1, 0.8, 0.6, 0.5, 0.4, 0.3, 0.2 };

        public ComputeTool(double width, double minDia)
        {
            if (width <= minDia)
                this.width = width;
            else
                this.width = minDia;
        }
        /// <summary>
        /// 获得开粗刀具
        /// </summary>
        /// <returns></returns>
        public string GetRoughTool()
        {
            if (width > 8.2)
                return "EM8";
            if (width > 6.2 && width <= 8.2)
                return "EM6";
            if (width <= 6.2)
                return "EM8";
            return "EM8";
        }
        /// <summary>
        /// 获得光基准框刀具
        /// </summary>
        /// <returns></returns>
        public string GetBaseStationTool()
        {
            if (width > 8.2)
                return "EM7.98";
            if (width > 6.2 && width <= 8.2)
                return "EM5.98";
            if (width <= 6.2)
                return "EM7.98";
            return "EM7.98";
        }
        /// <summary>
        /// 获得二次开粗刀具
        /// </summary>
        /// <returns></returns>
        public string GetTwiceRoughTool()
        {
            foreach (double k in twice)
            {
                if ((k + 0.2) <= width)
                    return "EM" + k.ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取精加工平刀
        /// </summary>
        /// <returns></returns>
        public string GetFinishFlatTool()
        {
            foreach (double k in twice)
            {
                if ((k + 0.2) <= width)
                    return "EM" + (k - 0.02).ToString();
            }
            return "EM2.98";
        }

        /// <summary>
        /// 获取精加工平刀
        /// </summary>
        /// <returns></returns>
        public string GetFinishBallTool()
        {
            foreach (double k in twice)
            {
                if ((k + 0.2) <= width)
                    return "BN" + (k - 0.02).ToString();
            }
            return "BN1.98";
        }


    }
}
