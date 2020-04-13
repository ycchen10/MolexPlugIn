using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class AnalyzeElectrode
    {
        private ElectrodeModel ele;


        public AnalyzeElectrode(ElectrodeModel ele)
        {
            this.ele = ele;
        }
        /// <summary>
        /// 获取最小距离值
        /// </summary>
        /// <returns></returns>
        public double GetMinDis()
        {
            double min = ele.EleInfo.EleMinDim;
            double minPHX = 9999;
            double minPHY = 9999;


            if (ele.EleInfo.PitchX != 0 && ele.EleInfo.PitchXNum != 1)
            {
                minPHX = ele.EleInfo.PitchX - 2 * ele.EleInfo.EleHeadDis[0];
            }
            if (ele.EleInfo.PitchY != 0 && ele.EleInfo.PitchYNum != 1)
            {
                minPHY = ele.EleInfo.PitchY - 2 * ele.EleInfo.EleHeadDis[1];
            }
            if (min >= minPHX && minPHX > 0)
                min = minPHX; 
            if (min >= minPHY && minPHY > 0)
                min = minPHY;
            return min;
        }
        /// <summary>
        ///分析体获得斜度和最下R角
        /// </summary>
        /// <returns></returns>
        public AnalyzeBuilder AnalyzeBody()
        {
            Body body = ele.PartTag.Bodies.ToArray()[0];
            AnalyzeBuilder builder = new AnalyzeBuilder(body);
            builder.Analyze(new Vector3d(0, 0, 1));
            builder.AnalyzeFaces.Sort();
            builder.AnalyzeFaces.RemoveRange(0, 5);
            builder.AnalyzeFaces.RemoveRange(1, 5);
            return builder;
        }

    }
}
