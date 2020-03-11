using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using Basic;

namespace MolexPlugin.Model
{
    public class ElectrodePichModel
    {
        public double XPich { get; private set; }

        public int XNumber { get; private set; }

        public double YPich { get; private set; }

        public int YNumber { get; private set; }

        public ElectrodeHeadModel HeadModel { get; private set; }
        /// <summary>
        /// 设定值
        /// </summary>    

        private PatternGeometry patternFeat;
        public ElectrodePichModel(double x, int xNumber, double y, int yNumber, ElectrodeHeadModel head)
        {
            this.XPich = x;
            this.XNumber = xNumber;
            this.YPich = y;
            this.YNumber = yNumber;
            this.HeadModel = head;
        }
        /// <summary>
        /// 创建阵列
        /// </summary>
        private void CreatePattern()
        {
            //  DeleExpression();
            if ((XNumber > 1 && Math.Abs(XPich) > 0) || (YNumber > 1 && Math.Abs(YPich) > 0))
            {
                this.patternFeat = PatternUtils.CreatePattern(this.XNumber.ToString(), this.XPich.ToString(), this.YNumber.ToString(),
                     this.YPich.ToString(), this.HeadModel.model.Work.Matr, this.HeadModel.model.Bodys.ToArray());
            }

        }
        /// <summary>
        /// 删除表达式
        /// </summary>
        private void DeleExpression()
        {
            ExpressionUtils.DeteteExp("xPitchDistance");
            ExpressionUtils.DeteteExp("xNCopies");
            ExpressionUtils.DeteteExp("yPitchDistance");
            ExpressionUtils.DeteteExp("yNCopies");
        }
        /// <summary>
        /// 更新阵列
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        public void UpdatePattern(double x, int xNumber, double y, int yNumber)
        {
            this.XPich = x;
            this.XNumber = xNumber;
            this.YPich = y;
            this.YNumber = yNumber;
            if (this.patternFeat == null)
            {
                CreatePattern();
                return;
            }

            ExpressionUtils.UpdateExp("xPitchDistance", this.XPich.ToString());
            ExpressionUtils.UpdateExp("xNCopies", this.XNumber.ToString());
            ExpressionUtils.UpdateExp("yPitchDistance", this.YPich.ToString());
            ExpressionUtils.UpdateExp("yNCopies", this.YNumber.ToString());
            DeleteObject.UpdateObject();

        }
        /// <summary>
        /// 删除阵列
        /// </summary>
        public void DelePattern()
        {
            if (patternFeat != null)
                DeleteObject.Delete(this.patternFeat);
            DeleExpression();
        }
        /// <summary>
        /// 设定值
        /// </summary>
        /// <returns></returns>
        public Point3d GetEleSetValue(bool datum)
        {
            Point3d setPt = GetSetPoint(HeadModel.GetEleMatr());
            double x, y, z;
            z = setPt.Z; ;
            if (datum)
            {
                x = Math.Round(setPt.X + (this.XNumber) * this.XPich / 2, 4);
                y = Math.Round(setPt.Y + (this.YNumber) * this.YPich / 2, 4);

                if (x >= y)
                {
                    y = Math.Round(setPt.Y + (this.YNumber - 1) * this.YPich / 2, 4);
                }
                else
                {
                    x = Math.Round(setPt.X + (this.XNumber - 1) * this.XPich / 2, 4);
                }
            }
            else
            {
                x = Math.Round(setPt.X + (this.XNumber - 1) * this.XPich / 2, 4);
                y = Math.Round(setPt.Y + (this.YNumber - 1) * this.YPich / 2, 4);

            }

            return new Point3d(x, y, z);
        }

        private Point3d GetSetPoint(Matrix4 eleMatr)
        {
            double anleZ = UMathUtils.Angle(eleMatr.GetZAxis(), HeadModel.model.Work.Matr.GetZAxis());
            double anleX = UMathUtils.Angle(eleMatr.GetZAxis(), HeadModel.model.Work.Matr.GetXAxis());
            double anleY = UMathUtils.Angle(eleMatr.GetZAxis(), HeadModel.model.Work.Matr.GetYAxis());
            if (UMathUtils.IsEqual(anleZ, Math.PI))
            {
                return new Point3d(Math.Ceiling(HeadModel.CenterPt.X), Math.Ceiling(HeadModel.CenterPt.Y), Math.Round(HeadModel.CenterPt.Z - HeadModel.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleX, 0))
            {
                return new Point3d(Math.Round(HeadModel.CenterPt.X + HeadModel.DisPt.X, 4), Math.Ceiling(HeadModel.CenterPt.Y), Math.Round(HeadModel.CenterPt.Z - HeadModel.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleX, Math.PI))
            {
                return new Point3d(Math.Round(HeadModel.CenterPt.X - HeadModel.DisPt.X, 4), Math.Ceiling(HeadModel.CenterPt.Y), Math.Round(HeadModel.CenterPt.Z - HeadModel.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleY, 0))
            {
                return new Point3d(Math.Ceiling(HeadModel.CenterPt.X), Math.Round(HeadModel.CenterPt.Y + HeadModel.DisPt.Y, 4), Math.Round(HeadModel.CenterPt.Z - HeadModel.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleY, Math.PI))
            {
                return new Point3d(Math.Ceiling(HeadModel.CenterPt.X), Math.Round(HeadModel.CenterPt.Y - HeadModel.DisPt.Y, 4), Math.Round(HeadModel.CenterPt.Z - HeadModel.DisPt.Z, 4));
            }
            return new Point3d();
        }
        /// <summary>
        /// 最大外形
        /// </summary>
        /// <returns></returns>
        public Point3d GetMaxOutline(bool datum)
        {
            double x, y, z;
            z = Math.Ceiling(2 * this.HeadModel.DisPt.Z + 20);
            if (datum)
            {
                x = Math.Ceiling(2 * this.HeadModel.DisPt.X + Math.Abs(this.XNumber) * this.XPich) + 2;
                y = Math.Ceiling(2 * this.HeadModel.DisPt.Y + Math.Abs(this.YNumber) * this.YPich) + 2;
                if (x >= y)
                {
                    y = Math.Ceiling(2 * this.HeadModel.DisPt.Y + Math.Abs((this.YNumber - 1) * this.YPich)) + 2;
                }
                else
                {
                    x = Math.Ceiling(2 * this.HeadModel.DisPt.X + Math.Abs((this.XNumber - 1) * this.XPich)) + 2;
                }
            }
            else
            {
                x = Math.Ceiling(2 * this.HeadModel.DisPt.X + Math.Abs((this.XNumber - 1) * this.XPich)) + 2;
                y = Math.Ceiling(2 * this.HeadModel.DisPt.Y + Math.Abs((this.YNumber - 1) * this.YPich)) + 2;
            }

            return new Point3d(x, y, z);
        }

    }
}
