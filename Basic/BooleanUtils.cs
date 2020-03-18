using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    public class BooleanUtils : ClassItem
    {
        /// <summary>
        /// 布尔操作
        /// </summary>
        /// <param name="targetBody">目标体</param>
        /// <param name="toolBody">工具体</param>
        /// <param name="copyTools">复制工具</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static NXOpen.Features.BooleanFeature CreateBooleanFeature(Body targetBody, Body toolBody, bool copyTools, NXOpen.Features.Feature.BooleanType type)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.BooleanFeature nullNXOpen_Features_BooleanFeature = null;
            NXOpen.Features.BooleanBuilder booleanBuilder1 = workPart.Features.CreateBooleanBuilderUsingCollector(nullNXOpen_Features_BooleanFeature);
            //ScCollector scCollector1 = booleanBuilder1.ToolBodyCollector;
            //  NXOpen.GeometricUtilities.BooleanRegionSelect booleanRegionSelect1 = booleanBuilder1.BooleanRegionSelect;            
            booleanBuilder1.CopyTools = copyTools;
            booleanBuilder1.Operation = type;

            bool added1 = booleanBuilder1.Targets.Add(targetBody);
            //NXOpen.TaggedObject[] targets1 = new NXOpen.TaggedObject[1];
            //targets1[0] = targetBody;
            //booleanRegionSelect1.AssignTargets(targets1);

            NXOpen.ScCollector scCollector = workPart.ScCollectors.CreateCollector();
            TaggedObject[] obj = { toolBody };
            SelectionRuleFactory fac = new SelectionRuleFactory(obj.ToList());
            //Body[] bodies1 = { toolBody };
            //BodyDumbRule bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
            //SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            //rules1[0] = bodyDumbRule1;
            scCollector.ReplaceRules(fac.CreateSelectionRule().ToArray(), false);

            booleanBuilder1.ToolBodyCollector = scCollector;

            //NXOpen.TaggedObject[] targets2 = new NXOpen.TaggedObject[1];
            //targets2[0] = toolBody;
            //booleanRegionSelect1.AssignTargets(targets2);

            try
            {
                NXOpen.Features.Feature boolFeature = booleanBuilder1.CommitFeature();
                return boolFeature as NXOpen.Features.BooleanFeature;
            }
           catch(Exception ex)
            {
                LogMgr.WriteLog("Basic.BooleanUtils.CreateBooleanFeature:错误：" + ex.Message);
                return null;
            }
            finally
            {
                booleanBuilder1.Destroy();
            }

           
      

        }


    }
}
