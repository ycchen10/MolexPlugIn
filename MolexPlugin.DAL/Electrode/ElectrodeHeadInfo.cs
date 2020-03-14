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

      

    }
}
