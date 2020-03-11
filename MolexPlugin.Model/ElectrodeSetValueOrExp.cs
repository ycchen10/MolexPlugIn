using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 获取电极设定值和表达式
    /// </summary>
    public class ElectrodeSetValueOrExp
    {

        private ElectrodeHeadModel head;

        private Matrix4 eleMatr;

        public ElectrodeSetValueOrExp(ElectrodeHeadModel head)
        {
            this.head = head;
            this.eleMatr = head.GetEleMatr();
        }


        public Point3d GetSetPoint()
        {
            double anleZ = UMathUtils.Angle(eleMatr.GetZAxis(), head.model.Work.Matr.GetZAxis());
            double anleX = UMathUtils.Angle(eleMatr.GetZAxis(), head.model.Work.Matr.GetXAxis());
            double anleY = UMathUtils.Angle(eleMatr.GetZAxis(), head.model.Work.Matr.GetXAxis());
            if (UMathUtils.IsEqual(anleZ, 0))
            {
                return new Point3d(Math.Ceiling(head.CenterPt.X), Math.Ceiling(head.CenterPt.Y), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleX, 0))
            {
                return new Point3d(Math.Round(head.CenterPt.X - head.DisPt.X, 4), Math.Ceiling(head.CenterPt.Y), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleX, Math.PI))
            {
                return new Point3d(Math.Round(head.CenterPt.X + head.DisPt.X, 4), Math.Ceiling(head.CenterPt.Y), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleY, 0))
            {
                return new Point3d(Math.Ceiling(head.CenterPt.X), Math.Round(head.CenterPt.Y - head.DisPt.Y, 4), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
            }
            if (UMathUtils.IsEqual(anleY, Math.PI))
            {
                return new Point3d(Math.Ceiling(head.CenterPt.X), Math.Round(head.CenterPt.Y + head.DisPt.Y, 4), Math.Round(head.CenterPt.Z - head.DisPt.Z, 4));
            }
            return new Point3d();
        }


    }
}
