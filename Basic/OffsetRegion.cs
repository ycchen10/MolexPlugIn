using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class OffsetRegion
    {
        /// <summary>
        ///偏置面
        /// </summary>
        /// <param name="side"></param>
        /// <param name="isok"></param>
        /// <param name="faces"></param>
        /// <returns></returns>
        public static NXObject Offset(double side, out bool isok, params Face[] faces)
        {
            isok = true;
           
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Session.UndoMarkId mark = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Offset");
            NXOpen.Features.AdmOffsetRegion nullNXOpen_Features_AdmOffsetRegion = null;
            NXOpen.Features.AdmOffsetRegionBuilder admOffsetRegionBuilder1;
            admOffsetRegionBuilder1 = workPart.Features.CreateAdmOffsetRegionBuilder(nullNXOpen_Features_AdmOffsetRegion);
            NXOpen.FaceDumbRule faceDumbRule;
            faceDumbRule = workPart.ScRuleFactory.CreateRuleFaceDumb(faces);

            NXOpen.SelectionIntentRule[] rules = new NXOpen.SelectionIntentRule[1];
            rules[0] = faceDumbRule;
            admOffsetRegionBuilder1.FaceToOffset.FaceCollector.ReplaceRules(rules, false);
            admOffsetRegionBuilder1.Distance.Value = side;
            NXOpen.NXObject nXObject1 = null;
            try
            {

                nXObject1 = admOffsetRegionBuilder1.Commit();
               
                return nXObject1;

            }
            catch (NXException ex)
            {
                isok = false;
                LogMgr.WriteLog("OffsetRegion:Offset:" + ex.Message);
                
                return null;
            }
            finally
            {
                admOffsetRegionBuilder1.Destroy();
                if(!isok)
                {
                    theSession.UndoToMark(mark, "Offset");
                }
                theSession.DeleteUndoMark(mark, "Offset");
            }
        }

    }
}
