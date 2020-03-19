using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;

namespace Basic
{
    /// <summary>
    /// 点
    /// </summary>
    public class PointUtils : ClassItem
    {
        public static Point CreatePointFeature(Point3d point)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.PointFeatureBuilder pointFeatureBuilder1;
            pointFeatureBuilder1 = workPart.BaseFeatures.CreatePointFeatureBuilder(nullNXOpen_Features_Feature);
            Point pt = workPart.Points.CreatePoint(point);
            pointFeatureBuilder1.Point = pt;

            try
            {
                NXOpen.Features.Feature pf = pointFeatureBuilder1.CommitFeature();
                return pf.GetEntities()[0] as Point;

            }
            catch
            {
                LogMgr.WriteLog("PointUtils:CreatePointFeature  创建点错误");
                return null;
            }
            finally
            {
                pointFeatureBuilder1.Destroy();

            }

        }
        public static Point CreatePoint(Point3d point)
        {
            Tag pointTag = Tag.Null;
            theUFSession.Curve.CreatePoint(new double[3] { point.X, point.Y, point.Z }, out pointTag);
            return NXObjectManager.Get(pointTag) as Point;
        }


    }
}
