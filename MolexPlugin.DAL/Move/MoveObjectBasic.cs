using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 移动工件
    /// </summary>
    public class MoveObjectBasic
    {
        private List<NXObject> selectObj;

        private Part workPart;
        private UFSession theUFSession;
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; private set; }
        /// <summary>
        /// 最大外形点
        /// </summary>
        public Point3d DisPt { get; private set; }
        public MoveObjectBasic(List<NXObject> objs)
        {
            this.selectObj = objs;
            this.workPart = Session.GetSession().Parts.Work;
            this.theUFSession = UFSession.GetUFSession();
            GetBoundingBox();
        }
        /// <summary>
        /// 创建外形点
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="disPt"></param>
        /// <returns></returns>
        public List<NXObject> CreatePoint()
        {
            if (UMathUtils.IsEqual(this.DisPt.X, 0) && UMathUtils.IsEqual(this.DisPt.Y, 0) && UMathUtils.IsEqual(this.DisPt.Y, 0))
                return null;
            double[] x = { CenterPt.X - DisPt.X, CenterPt.X, CenterPt.X + DisPt.X };
            double[] y = { CenterPt.Y - DisPt.Y, CenterPt.Y, CenterPt.Y + DisPt.Y };
            double[] z = { CenterPt.Z - DisPt.Z, CenterPt.Z, CenterPt.Z + DisPt.Z };
            Matrix4 mat = new Matrix4();
            List<NXObject> points = new List<NXObject>();
            mat.Identity();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Point3d temp = new Point3d(x[i], y[j], z[k]);
                        mat.ApplyPos(ref temp);
                        Tag pointTag = Tag.Null;
                        theUFSession.Curve.CreatePoint(new double[] { temp.X, temp.Y, temp.Z }, out pointTag);
                        theUFSession.Obj.SetColor(pointTag, 186);
                        points.Add(NXObjectManager.Get(pointTag) as NXObject);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 获取点到面的投影点
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public Point3d GetPointToFaceDis(Point pt, Face face)
        {
            FaceData faceData = FaceUtils.AskFaceData(face);
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(faceData.Point, faceData.Dir);
            Point3d temp = pt.Coordinates;
            mat.ApplyPos(ref temp);
            Matrix4 invers = mat.GetInversMatrix();
            temp.Z = 0;
            invers.ApplyPos(ref temp);
            return temp;
        }
        /// <summary>
        /// 获取最大外形
        /// </summary>
        private void GetBoundingBox()
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(selectObj.ToArray(), null, mat, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
        }
        /// <summary>
        /// WCS移动到ABS
        /// </summary>
        /// <param name="objs"></param>
        public bool MoveObjectWcsToAbs()
        {
            CoordinateSystem wcs = Session.GetSession().Parts.Work.WCS.CoordinateSystem;
            return Basic.MoveObject.MoveObjectOfCsys(wcs, this.selectObj.ToArray()) != null;
        }
        /// <summary>
        /// 移动到中心最高点
        /// </summary>
        /// <returns></returns>
        public bool MoveObjectMaxCenterPoint()
        {
            Point3d maxPt = new Point3d(this.CenterPt.X, this.CenterPt.Y, (this.CenterPt.Z + this.DisPt.Z));
            Point3d startPt = new Point3d(0, 0, 0);
            return MoveObject.MoveObjectOfPointToPoint(startPt, maxPt,this.selectObj.ToArray()) != null;
        }
        /// <summary>
        /// 移动到中心点最低点
        /// </summary>
        /// <returns></returns>
        public bool MoveObjectMinCenterPoint()
        {
            Point3d maxPt = new Point3d(this.CenterPt.X, this.CenterPt.Y, (this.CenterPt.Z - this.DisPt.Z));
            Point3d startPt = new Point3d(0, 0, 0);
            return MoveObject.MoveObjectOfPointToPoint(startPt, maxPt, this.selectObj.ToArray()) != null;
        }

         public bool MoveObjectRotate(Vector3d axis,double angle)
        {
            return MoveObject.MoveObjectOfRotate(axis, angle, this.selectObj.ToArray()) != null;
        }

    }
}
