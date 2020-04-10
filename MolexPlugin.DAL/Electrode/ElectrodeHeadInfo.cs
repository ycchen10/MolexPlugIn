using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极头信息
    /// </summary>
    public class ElectrodeHeadInfo
    {
        public CreateConditionModel ConditionModel { get; private set; }

        public Point3d CenterPt { get; private set; }

        public Point3d DisPt { get; private set; }
        public ElectrodeHeadInfo(CreateConditionModel model)
        {
            this.ConditionModel = model;
            GetHeadAttr();
        }
        /// <summary>
        /// 获取中心点
        /// </summary>
        private void GetHeadAttr()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Matrix4 matr = this.ConditionModel.Work.WorkMatr;
            Matrix4 invers = matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(matr, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(this.ConditionModel.Bodys.ToArray(), csys, this.ConditionModel.Work.WorkMatr, ref centerPt, ref disPt);
            this.DisPt = disPt;
            this.CenterPt = centerPt;
        }
        /// <summary>
        /// 查询电极头最小距离
        /// </summary>
        /// <returns></returns>
        public double AskMinDim()
        {
            double min = 9999;
            double[] pt1 = new double[3];
            double[] pt2 = new double[3];
            for (int i = 0; i < this.ConditionModel.Bodys.Count - 1; i++)
            {
                for (int j = i + 1; j < this.ConditionModel.Bodys.Count; j++)
                {
                    double temp = AnalysisUtils.AskMinimumDist(this.ConditionModel.Bodys[i].Tag, this.ConditionModel.Bodys[j].Tag, out pt1, out pt2);
                    temp = Math.Round(temp, 3);
                    if (min >= temp)
                        min = temp;
                }
            }
            return min;
        }

    }
}
