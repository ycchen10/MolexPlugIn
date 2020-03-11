using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    public class ExtrudedUtils
    {
        /// <summary>
        /// 创建拉伸特征
        /// </summary>
        /// <param name="vec">向量</param>
        /// <param name="start">起始</param>
        /// <param name="end">终止</param>
        /// <param name="extrude">lastez</param>
        /// <param name="line">线</param>
        /// <returns></returns>
        public static NXOpen.Features.Feature CreateExtruded(Vector3d vec, string start, string end, NXOpen.Features.Feature extrude = null, params TaggedObject[] line)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            SelectionRuleFactory rules = new SelectionRuleFactory(line.ToList());
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.ExtrudeBuilder extrudeBuilder1 = workPart.Features.CreateExtrudeBuilder(nullNXOpen_Features_Feature);
            NXOpen.Section section1 = workPart.Sections.CreateSection();
            extrudeBuilder1.Section = section1;
            extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = start;
            extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = end;

            NXOpen.Point3d origin1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            NXOpen.Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vec, NXOpen.SmartObject.UpdateOption.WithinModeling);


            NXOpen.NXObject nullNXOpen_NXObject = null;

            section1.AddToSection(rules.CreateSelectionRule().ToArray(), (NXObject)line[0], nullNXOpen_NXObject, nullNXOpen_NXObject, origin1, NXOpen.Section.Mode.Create, false);
            extrudeBuilder1.Direction = direction1;
            NXOpen.Session.UndoMarkId markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start Extruded");
            try
            {

                return extrudeBuilder1.CommitFeature();
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("ExtrudedUtils:CreateExtruded:" + ex.Message);
                return null;
            }
            finally
            {
                extrudeBuilder1.Destroy();
                theSession.UpdateManager.DoUpdate(markId);
                theSession.DeleteUndoMark(markId, "End Extruded");

            }
        }
    }
}
