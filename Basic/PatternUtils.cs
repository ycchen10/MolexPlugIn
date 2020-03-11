using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;

namespace Basic
{
    public class PatternUtils : ClassItem
    {

        /// <summary>
        /// 阵列几何特征
        /// </summary>
        /// <param name="obj">阵列对象</param>
        /// <param name="xNCopies">x方向个数</param>
        /// <param name="xPitchDistance">x方向尺寸</param>
        /// <param name="yNCopies">Y方向个数</param>
        /// <param name="yPitchDistance">Y方向尺寸</param>
        /// <returns></returns>
        public static PatternGeometry CreatePattern(string xNCopies, string xPitchDistance, string yNCopies, string yPitchDistance, Matrix4 mat = null, params DisplayableObject[] obj)
        {
            Part workPart = theSession.Parts.Work;
            if (mat == null)
            {
                mat = new Matrix4();
                mat.Identity();
                mat.TransformToCsys(workPart.WCS.CoordinateSystem, ref mat);
            }
            NXOpen.Features.PatternGeometry nullNXOpen_Features_PatternGeometry = null;
            NXOpen.Features.PatternGeometryBuilder patternGeometryBuilder1;
            patternGeometryBuilder1 = workPart.Features.CreatePatternGeometryBuilder(nullNXOpen_Features_PatternGeometry);
            Direction xDirection;
            Direction yDirection;
            Vector3d x = mat.GetXAxis();
            Vector3d y = mat.GetYAxis();
            Point3d origin = new Point3d(0, 0, 0);
            Matrix4 invers = mat.GetInversMatrix();
            invers.ApplyPos(ref origin);
            invers.ApplyVec(ref x);
            invers.ApplyVec(ref y);
            bool added1 = patternGeometryBuilder1.GeometryToPattern.Add(obj); //设置要阵列的对象

            patternGeometryBuilder1.PatternService.RectangularDefinition.UseYDirectionToggle = true;

            patternGeometryBuilder1.ReferencePoint.Point = workPart.Points.CreatePoint(origin); //指定参考点

            xDirection = workPart.Directions.CreateDirection(origin, x, SmartObject.UpdateOption.WithinModeling);   //方向
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.NCopies.RightHandSide = xNCopies;  //要阵列的个数（包括本身）
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.NCopies.SetName("xNCopies");
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.PitchDistance.RightHandSide = xPitchDistance; //设置节距
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.PitchDistance.SetName("xPitchDistance");
            patternGeometryBuilder1.PatternService.RectangularDefinition.XDirection = xDirection;

            yDirection = workPart.Directions.CreateDirection(origin, y, SmartObject.UpdateOption.WithinModeling);   //方向
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.NCopies.SetName("yNCopies");
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.PitchDistance.SetName("yPitchDistance");
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.NCopies.RightHandSide = yNCopies;  //要阵列的个数（包括本身）
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.PitchDistance.RightHandSide = yPitchDistance; //设置节距
            patternGeometryBuilder1.PatternService.RectangularDefinition.YDirection = yDirection;

            try
            {

                return patternGeometryBuilder1.CommitFeature() as PatternGeometry;

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PatternUtils:CreatePattern:      " + ex.Message);
                return null;
            }
            finally
            {
                patternGeometryBuilder1.Destroy();
            }

        }
    }
}
