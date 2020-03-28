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
    public class ZPositiveElectrodeSetValue : AbstractElectrodeSetValue
    {
        public ZPositiveElectrodeSetValue(ElectrodeHeadInfo head) : base(head)
        {
        }

        public override void CreateExp(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            base.CreateDefault(zDatum);
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveX"), "-(xNCopies-1)*xPitchDistance/2");
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveY"), "-(yNCopies-1)*yPitchDistance/2");
            if (zDatum)
            {
                double[] pre = GetPreparation(x, xNumber, y, yNumber, zDatum);
                if (pre[0] >= pre[1])
                {
                    ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveX"), "-(xNCopies-2)*xPitchDistance/2");
                    ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveBoxX"), "-(xNCopies)*xPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveY"), "-(yNCopies-2)*yPitchDistance/2");
                    ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("moveBoxY"), "-(yNCopies)*yPitchDistance/2");
                }
            }
        }

        public override Matrix4 GetEleMatr(int[] pre, Point3d center)
        {
            Matrix4 workMat = this.head.ConditionModel.Work.WorkMatr;
            Matrix4 workInvers = workMat.GetInversMatrix();
            workInvers.ApplyPos(ref center);
            Vector3d vecY = workMat.GetYAxis();
            vecY.X = -vecY.X;
            vecY.Y = -vecY.Y;
            vecY.Z = -vecY.Z;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(center, workMat.GetXAxis(), vecY);      
            return mat;
        }



        public override double[] GetPreparation(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            double preX = Math.Ceiling(2 * this.head.DisPt.X + Math.Abs((xNumber - 1) * x)) + 2;
            double preY = Math.Ceiling(2 * this.head.DisPt.Y + Math.Abs((yNumber - 1) * y)) + 2;
            double preZ = Math.Ceiling(2 * this.head.DisPt.Z) + 20;
            if (zDatum)
            {
                if (preX >= preY)
                {
                    preX = Math.Ceiling(2 * this.head.DisPt.X + Math.Abs((xNumber) * x)) + 2;
                }
                else
                {
                    preY = Math.Ceiling(2 * this.head.DisPt.Y + Math.Abs((yNumber) * y)) + 2;
                }
            }
            return new double[3] { preX, preY, preZ };
        }

        public override Point3d GetSingleHeadSetValue()
        {
            return new Point3d(Math.Round(head.CenterPt.X,3), Math.Round(head.CenterPt.Y,3), Math.Round(head.CenterPt.Z - head.DisPt.Z, 3));
        }

        public override double GetZHeight(double exp)
        {
            return Math.Abs(head.CenterPt.Z - head.DisPt.Z) + exp;
        }

    }
}
