using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 修剪实体
    /// </summary>
    public class TrimBodyUtils : ClassItem
    {
        /// <summary>
        /// 创建修剪特征
        /// </summary>
        /// <param name="body"></param>
        /// <param name="planeFace">平面</param>
        /// <param name="trimBodyFeature"></param>
        /// <returns></returns>
        public static NXOpen.Features.TrimBody2  CreateTrimBodyFeature(Face planeFace,bool isFlip, out bool isok, params Body[] bodys)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.TrimBody2 nullNXOpen_Features_TrimBody2 = null;
            NXOpen.Features.TrimBody2Builder trimBody2Builder1 = workPart.Features.CreateTrimBody2Builder(nullNXOpen_Features_TrimBody2);
            List<TaggedObject> tgs = new List<TaggedObject>();
            foreach (Body body in bodys)
            {
                tgs.Add(body);
            }
            SelectionRuleFactory fac = new SelectionRuleFactory(tgs);

            NXOpen.Plane plane1 = PlaneUtils.CreatePlaneOfFace(planeFace, isFlip);
            trimBody2Builder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;
            trimBody2Builder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.NewPlane;
            NXOpen.ScCollector scCollector1 = workPart.ScCollectors.CreateCollector();
            scCollector1.ReplaceRules(fac.CreateSelectionRule().ToArray(), false);
            trimBody2Builder1.TargetBodyCollector = scCollector1;
            
            try
            {
                isok = true;
                return  trimBody2Builder1.CommitFeature() as NXOpen.Features.TrimBody2;
               
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.TrimBody:CreateTrimBodyFeature:" + ex.Message);
                isok = false;
                return null;
            }
            finally
            {
                trimBody2Builder1.Destroy();
            }

        }


    }
}
