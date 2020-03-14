using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// Z+向加工
    /// </summary>
    public class XPositiveElectrodeSetValue : AbstractElectrodeSetValue
    {
        public XPositiveElectrodeSetValue(ElectrodeHeadInfo head) : base(head)
        {
        }

        public override void CreateExp(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            base.CreateDefault(zDatum);
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yPitchDistance"), "PitchY");
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveY"), "-(yNCopies-1)*yPitchDistance/2");
            if (zDatum)
            {
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveY"), "-(yNCopies-2)*yPitchDistance/2");
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveBoxY"), "-(yNCopies)*yPitchDistance/2");

            }
        }

        public override Matrix4 GetEleMatr(int[] pre, Point3d center)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();          
            center.Z = center.Z + (pre[0] / 2 - 0.7);
            Matrix4 workMat = this.head.ConditionModel.Work.WorkMatr;
            Matrix4 workInvers = workMat.GetInversMatrix();
            workInvers.ApplyPos(ref center);
            UVector vecX = new UVector();
            UVector vecY = new UVector();
            UVector vecZ = new UVector();
            workMat.GetYAxis(ref vecY);
            workMat.GetXAxis(ref vecZ);
            vecX = vecY ^ vecZ;
            UVector point = new UVector(center.X, center.Y, center.Z);
            mat.TransformToZAxis(point, vecX, vecY);
            return mat;
        }



        public override double[] GetPreparation(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            double preX = Math.Ceiling(2 * this.head.DisPt.Z) + 2;
            double preY = Math.Ceiling(2 * this.head.DisPt.Y + Math.Abs((yNumber - 1) * y)) + 2;
            double preZ = Math.Ceiling(2 * this.head.DisPt.X) + 20;
            if (zDatum)
            {
                preY = Math.Ceiling(2 * this.head.DisPt.Y + Math.Abs((yNumber) * y));
            }
            return new double[3] { preX, preY, preZ };
        }

        public override Point3d GetSingleHeadSetValue()
        {
            return new Point3d(Math.Round(head.CenterPt.X + head.DisPt.X, 4), Math.Ceiling(head.CenterPt.Y), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
        }

        public override double GetZHeight(double exp)
        {
            return head.DisPt.X * 2 + exp;
        }
    }
}
