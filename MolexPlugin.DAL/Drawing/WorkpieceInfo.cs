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
    public class WorkpieceInfo
    {
        public Part workpiece { get; }
        private WorkModel work;
        public Point3d CenterPt { get; private set; }
        public Point3d DisPt { get; private set; }
        private Point[] poin = new Point[2];
        public NXOpen.Assemblies.Component workpieceComp { get; private set; }
        public WorkpieceInfo(Part workpiece, WorkModel work)
        {
            this.workpiece = workpiece;
            this.work = work;
            workpieceComp = AssmbliesUtils.GetPartComp(this.work.PartTag, this.workpiece);
            this.poin = CreateMinAndMaxPt();
        }
        /// <summary> 
        /// 创建外形点
        /// </summary>
        /// <returns></returns>
        private Point[] CreateMinAndMaxPt()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Point[] pt = new Point[2];
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();

            List<Body> bodys = new List<Body>();
            foreach (Body body in this.workpiece.Bodies.ToArray())
            {
                bodys.Add(AssmbliesUtils.GetNXObjectOfOcc(this.workpieceComp.Tag, body.Tag) as Body);
            }
            //  Body[] bodys = this.workpiece.Bodies.ToArray(); 
            Matrix4 invers = this.work.WorkMatr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.WorkMatr, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(bodys.ToArray(), csys, this.work.WorkMatr, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
            Point3d minPt = new Point3d(centerPt.X - disPt.X, centerPt.Y - disPt.Y, centerPt.Z - disPt.Z);
            Point3d maxPt = new Point3d(centerPt.X + disPt.X, centerPt.Y + disPt.Y, centerPt.Z + disPt.Z);

            invers.ApplyPos(ref maxPt);
            invers.ApplyPos(ref minPt);
            // PartUtils.SetPartWork(workpieceComp);
            pt[0] = PointUtils.CreatePoint(minPt);
            theUFSession.Obj.SetLayer(pt[0].Tag, 254);
            pt[1] = PointUtils.CreatePoint(maxPt);
            theUFSession.Obj.SetLayer(pt[1].Tag, 254);
            // PartUtils.SetPartWork(null);
            return pt;
        }
        /// <summary>
        /// 获取隐藏
        /// </summary>
        /// <returns></returns>
        public NXOpen.Assemblies.Component[] GetHideComp()
        {
            List<NXOpen.Assemblies.Component> hides = new List<NXOpen.Assemblies.Component>();
            NXOpen.Assemblies.Component workComp = work.PartTag.ComponentAssembly.RootComponent;
            foreach (NXOpen.Assemblies.Component ct in workComp.GetChildren())
            {

                NXOpen.Assemblies.Component[] edm = ct.GetChildren();
                if (edm.Length != 0)
                {
                    ct.Unblank();
                    foreach (NXOpen.Assemblies.Component cp in edm)
                    {
                        cp.Unblank();
                        if (!cp.Name.Equals(this.workpiece.Name))
                            hides.Add(cp);
                    }
                }
                else
                    hides.Add(ct);
            }
            return hides.ToArray();
        }

        public Point[] GetPointOcc()
        {
            Point[] ptComp = new Point[2];
            //ptComp[0] = AssmbliesUtils.GetNXObjectOfOcc(workpieceComp.Tag, poin[0].Tag) as Point;
            //ptComp[1] = AssmbliesUtils.GetNXObjectOfOcc(workpieceComp.Tag, poin[1].Tag) as Point;

            ptComp[0] = poin[0];
            ptComp[1] = poin[1];

            return ptComp;
        }

    }
}
