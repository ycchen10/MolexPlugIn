using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Basic
{
    /// <summary>
    /// 设置CSYS
    /// </summary>
    public class CsysUtils : ClassItem
    {

        public static CartesianCoordinateSystem CreateCsys(Matrix4 matr, Point3d origin)
        {
            Part workPart = theSession.Parts.Work;
            return workPart.CoordinateSystems.CreateCoordinateSystem(origin, matr.GetXAxis(), matr.GetYAxis());
        }
        /// <summary>
        /// 以点设置WCS
        /// </summary>
        /// <param name="centerPt"></param>
        /// <returns></returns>
        public static void SetWcsOfCentePoint(Point3d centerPt)
        {
            Part workPart = theSession.Parts.Work;
            CoordinateSystem csys = workPart.WCS.CoordinateSystem;
            NXOpen.WCS wcs = workPart.WCS;
            wcs.SetOriginAndMatrix(centerPt, csys.Orientation.Element);
        }
        /// <summary>
        /// 以点和3x3的矩阵提到WCS
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="matr"></param>
        public static void SetWcsOfCenteAndMatr(Point3d centerPt, Matrix3x3 matr)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.WCS wcs = workPart.WCS;
            wcs.SetOriginAndMatrix(centerPt, matr);
        }
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public static void RotateWcs(NXOpen.WCS.Axis axis, double angle)
        {
            Part workPart = theSession.Parts.Work;
            workPart.WCS.Rotate(axis, angle);
        }
        /// <summary>
        /// 把WCS移动ABS
        /// </summary>
        public static void SetWcsToAbs()
        {
            Part workPart = theSession.Parts.Work;
            Point3d centerPt = new Point3d(0, 0, 0);
            NXOpen.WCS wcs = workPart.WCS;
            Matrix3x3 matrix1;
            matrix1.Xx = 1.0;
            matrix1.Xy = 0.0;
            matrix1.Xz = 0.0;
            matrix1.Yx = 0.0;
            matrix1.Yy = 1.0;
            matrix1.Yz = 0.0;
            matrix1.Zx = 0.0;
            matrix1.Zy = 0.0;
            matrix1.Zz = 1.0;
            wcs.SetOriginAndMatrix(centerPt, matrix1);
            wcs.Visibility = true;
        }


    }
}
