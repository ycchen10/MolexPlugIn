using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 边界
    /// </summary>
    public class BoundaryModel : IComparable<BoundaryModel>
    {
        /// <summary>
        /// 边界
        /// </summary>
        public List<Edge> Edges { get; set; } = new List<Edge>();
        /// <summary>
        /// 边界最高点
        /// </summary>
        public Point3d BouudaryPt { get; set; }
        /// <summary>
        /// 边界开放还是封闭
        /// </summary>
        public BoundarySet.BoundaryTypes Types { get; set; }
        /// <summary>
        /// 刀具侧
        /// </summary>
        public BoundarySet.ToolSideTypes ToolSide { get; set; }

        private Point3d disPt = new Point3d(0, 0, 0);
        private Point3d centerPt = new Point3d(0, 0, 0);
        public Point3d DisPt
        {
            get
            {
                if (UMathUtils.IsEqual(disPt, new Point3d(0, 0, 0)))
                    GetBoundingBox();
                return disPt;
            }

        }

        public Point3d CenterPt
        {
            get
            {
                if (UMathUtils.IsEqual(disPt, new Point3d(0, 0, 0)))
                    GetBoundingBox();
                return centerPt;
            }
        }
         private void GetBoundingBox()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            BoundingBoxUtils.GetBoundingBoxInLocal(Edges.ToArray(), null, mat, ref centerPt, ref disPt);
        }

        public int CompareTo(BoundaryModel other)
        {
            if (!UMathUtils.IsEqual(this.CenterPt.X, other.CenterPt.X))
                return this.CenterPt.X.CompareTo(other.CenterPt.X);
            else
                return this.CenterPt.Y.CompareTo(other.CenterPt.Y);
        }


    }
}
 