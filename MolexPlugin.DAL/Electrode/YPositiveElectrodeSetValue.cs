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
    /// Y+向加工
    /// </summary>
    public class YPositiveElectrodeSetValue : AbstractElectrodeSetValue
    {
        public YPositiveElectrodeSetValue(ElectrodeHeadInfo head) : base(head)
        {
        }

        public override void CreateExp(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            base.CreateDefault(zDatum);
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveX"), "-(xNCopies-1)*xPitchDistance/2");
            if (zDatum)
            {
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveX"), "-(xNCopies-2)*xPitchDistance/2");
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveBoxX"), "-(xNCopies)*xPitchDistance/2");

            }
        }

        public override Matrix4 GetEleMatr(int[] pre, Point3d center)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();        
            center.Z = center.Z + (pre[1] / 2 - 1.2);
            Matrix4 workMat = this.head.ConditionModel.Work.WorkMatr;
            Matrix4 workInvers = workMat.GetInversMatrix();
            workInvers.ApplyPos(ref center);
            UVector vecX = new UVector();
            UVector vecY = new UVector();
            UVector vecZ = new UVector();
            workMat.GetYAxis(ref vecZ);
            workMat.GetXAxis(ref vecX);
            vecY = vecX ^ vecZ;
            UVector point = new UVector(center.X, center.Y, center.Z);
            mat.TransformToZAxis(point, vecX, vecY);
            return mat;
        }

   

        public override double[] GetPreparation(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            double preY = Math.Ceiling(2 * this.head.DisPt.Z) + 2;
            double preX = Math.Ceiling(2 * this.head.DisPt.X + Math.Abs((xNumber - 1) * x)) + 2;
            double preZ = Math.Ceiling(2 * this.head.DisPt.Y) + 20;
            if (zDatum)
            {
                preX = Math.Ceiling(2 * this.head.DisPt.X + Math.Abs((xNumber) * x)) + 2;
            }
            return new double[3] { preX, preY, preZ };
        }

        public override Point3d GetSingleHeadSetValue()
        {
            return new Point3d(Math.Round(head.CenterPt.X,3), Math.Round(head.CenterPt.Y - head.DisPt.Y, 3), Math.Round(head.CenterPt.Z - head.DisPt.Z, 3));
        }

        public override double GetZHeight(double exp)
        {
            return head.DisPt.Y * 2 + exp;
        }
    }
}
