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
    public class WorkpieceInfo
    {
        private Part workpiece;
        private WorkModel work;
        public Point3d CenterPt { get; private set; }
        public Point3d DisPt { get; private set; }
        public Point[] Points { get; private set; }
        public WorkpieceInfo(Part workpiece, WorkModel work)
        {
            this.workpiece = workpiece;
            this.work = work;
            this.Points = CreateMinAndMaxPt();
        }
        /// <summary>
        /// 创建外形点
        /// </summary>
        /// <returns></returns>
        private Point[] CreateMinAndMaxPt()
        {
            Point[] pt = new Point[2];
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Body[] bodys = this.workpiece.Bodies.ToArray();
            Matrix4 invers = this.work.WorkMatr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.WorkMatr, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(bodys, csys, this.work.WorkMatr, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
            Point3d minPt = new Point3d(centerPt.X - disPt.X, centerPt.Y - disPt.Y, centerPt.Z - disPt.Z);
            Point3d maxPt = new Point3d(centerPt.X + disPt.X, centerPt.Y + disPt.Y, centerPt.Z + disPt.Z);
            invers.ApplyPos(ref maxPt);
            invers.ApplyPos(ref minPt);
            pt[0] = workpiece.Points.CreatePoint(minPt);
            pt[1] = workpiece.Points.CreatePoint(maxPt);
            return pt;
        }

        public NXOpen.Assemblies.Component[] GetHideComp()
        {
            List<NXOpen.Assemblies.Component> hides = new List<NXOpen.Assemblies.Component>();
            NXOpen.Assemblies.Component workComp = work.PartTag.ComponentAssembly.RootComponent;
            foreach (NXOpen.Assemblies.Component ct in workComp.GetChildren())
            {
                hides.Add(ct);
                NXOpen.Assemblies.Component[] edm = ct.GetChildren();
                if (edm.Length != 0)
                {
                    foreach (NXOpen.Assemblies.Component cp in edm)
                    {
                        if (!cp.Name.Equals(this.workpiece.Name))
                            hides.Add(cp);
                    }
                }
            }
            return hides.ToArray();
        }

    }
}
