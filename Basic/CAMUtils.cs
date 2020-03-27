using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Basic
{

    public class CAMUtils
    {
        /// <summary>
        /// 创建加工体
        /// </summary>
        /// <param name="workpieceName"></param>
        /// <param name="bodys"></param>
        public static void CreateFeatureGeometry(string workpieceName, params Body[] bodys)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.CAM.OrientGeometry orientGeometry1 = (NXOpen.CAM.OrientGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject("MCS_MILL");
            if (orientGeometry1 == null)
            {
                LogMgr.WriteLog("无法找到MCS_MILL加工坐标");
            }

            NXOpen.CAM.NCGroup nCGroup1;
            nCGroup1 = workPart.CAMSetup.CAMGroupCollection.CreateGeometry(orientGeometry1, "mill_planar", "WORKPIECE", NXOpen.CAM.NCGroupCollection.UseDefaultName.True, workpieceName);
            NXOpen.CAM.FeatureGeometry featureGeometry1 = (NXOpen.CAM.FeatureGeometry)nCGroup1;
            NXOpen.CAM.MillGeomBuilder millGeomBuilder1;
            millGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillGeomBuilder(featureGeometry1);

            NXOpen.CAM.GeometrySetList geometrySetList1;
            geometrySetList1 = millGeomBuilder1.PartGeometry.GeometryList;


            NXOpen.TaggedObject taggedObject1;
            taggedObject1 = geometrySetList1.FindItem(0);

            NXOpen.CAM.GeometrySet geometrySet1 = (NXOpen.CAM.GeometrySet)taggedObject1;

            NXOpen.BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodys, true);

            NXOpen.ScCollector scCollector1;
            scCollector1 = geometrySet1.ScCollector;

            NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);

            millGeomBuilder1.BlankGeometry.BlankDefinitionType = NXOpen.CAM.GeometryGroup.BlankDefinitionTypes.AutoBlock;

            try
            {
                NXOpen.NXObject nXObject2;
                nXObject2 = millGeomBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("CAMUtils:CreateFeatureGeometry:" + ex.Message);
            }
            finally
            {
                millGeomBuilder1.Destroy();
            }
        }
        /// <summary>
        /// 设置加工体
        /// </summary>
        /// <param name="workpiece"></param>
        /// <param name="bodys"></param>
        public static void SetFeatureGeometry(string workpiece, params Body[] bodys)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.CAM.FeatureGeometry featureGeometry1 = (NXOpen.CAM.FeatureGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject(workpiece);
            if (featureGeometry1 == null)
            {
                LogMgr.WriteLog("无法找到"+workpiece+"加工体");
            }
            NXOpen.CAM.MillGeomBuilder millGeomBuilder1;
            millGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillGeomBuilder(featureGeometry1);
            NXOpen.CAM.GeometrySetList geometrySetList1;
            geometrySetList1 = millGeomBuilder1.PartGeometry.GeometryList;
            NXOpen.TaggedObject taggedObject1;
            taggedObject1 = geometrySetList1.FindItem(0);
            NXOpen.CAM.GeometrySet geometrySet1 = (NXOpen.CAM.GeometrySet)taggedObject1;
            NXOpen.BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodys, true);

            NXOpen.ScCollector scCollector1;
            scCollector1 = geometrySet1.ScCollector;

            NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = millGeomBuilder1.Commit();
            }
          catch(Exception ex)
            {
                LogMgr.WriteLog("CAMUtils:SetFeatureGeometry:" + ex.Message);
            }
            finally
            {
                millGeomBuilder1.Destroy();
            }
       
        }

    }
}
