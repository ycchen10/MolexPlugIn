using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 点
    /// </summary>
    public class PointUtils : ClassItem
    {
        public static NXObject CreatePointFeature(Point point)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.PointFeatureBuilder pointFeatureBuilder1;
            pointFeatureBuilder1 = workPart.BaseFeatures.CreatePointFeatureBuilder(nullNXOpen_Features_Feature);
            pointFeatureBuilder1.Point = point;
            NXOpen.NXObject nXObject1;
            try
            {
                nXObject1 = pointFeatureBuilder1.Commit();
                return nXObject1;
            }
            catch
            {
                LogMgr.WriteLog("CycPointUtils:CreatePointFeature  创建点错误");
            }
            finally
            {
                pointFeatureBuilder1.Destroy();

            }
            return null;
        }

    }
}
