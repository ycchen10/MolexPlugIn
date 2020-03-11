using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;

namespace MolexPlugin.Model
{
    public class ElectrodeHeadModel
    {
        private Part workPart;

        public EleConditionModel model { get; private set; }

        public Point3d CenterPt { get; private set; }

        public Point3d DisPt { get; private set; }

        public ElectrodeHeadModel(EleConditionModel model)
        {
            workPart = Session.GetSession().Parts.Work;
            this.model = model;
            GetHeadAttr();
        }
        /// <summary>
        /// 获取中心点
        /// </summary>
        private void GetHeadAttr()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Matrix4 matr = this.model.Work.Matr;
            Matrix4 invers = matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(matr, invers);           
            BoundingBoxUtils.GetBoundingBoxInLocal(this.model.Bodys.ToArray(), csys, this.model.Work.Matr, ref centerPt, ref disPt);
            this.DisPt = disPt;
            this.CenterPt = centerPt;
        }
        /// <summary>
        /// 获取电极矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetEleMatr()
        {
            double anle = UMathUtils.Angle(this.model.Work.Matr.GetZAxis(), this.model.Vec);
            Matrix4 mat;
            if (UMathUtils.IsEqual(anle, 0))
            {
                mat = new Matrix4(this.model.Work.Matr);
                mat.RolateWithX(Math.PI);
                return mat;
            }
            else
            {
                UVector zVec = new UVector(-this.model.Vec.X, -this.model.Vec.Y, -this.model.Vec.Z);
                UVector xVec = new UVector();
                UVector yVec = new UVector();
                Matrix4 inver = this.model.Work.Matr.GetInversMatrix();

                UVector orinig = new UVector(CenterPt.X, CenterPt.Y, CenterPt.Z);
                inver.ApplyPos(ref orinig);
                double anle1 = UMathUtils.Angle(this.model.Work.Matr.GetXAxis(), this.model.Vec);
                if (UMathUtils.IsEqual(anle1, 0) || UMathUtils.IsEqual(anle1, Math.PI))
                {
                    this.model.Work.Matr.GetYAxis(ref yVec);
                    xVec = yVec ^ zVec;
                }
                else
                {
                    this.model.Work.Matr.GetXAxis(ref xVec);
                    yVec = xVec ^ zVec;
                }
                mat = new Matrix4();
                mat.Identity();
                mat.TransformToZAxis(orinig, xVec, yVec);
                return mat;
            }
        }


    }
}
