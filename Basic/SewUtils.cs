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
    /// 缝合
    /// </summary>
    public class SewUtils
    {
        public static NXOpen.Features.Sew SewFeature(Body body1, params Body[] body2s)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.SewBuilder sewBuilder1;
            sewBuilder1 = workPart.Features.CreateSewBuilder(nullNXOpen_Features_Feature);
            sewBuilder1.OutputMultipleSheets = true;
            bool added1;
            added1 = sewBuilder1.TargetBodies.Add(body1);
            bool added2;
            added2 = sewBuilder1.ToolBodies.Add(body2s);
            try
            {

                return sewBuilder1.CommitFeature() as NXOpen.Features.Sew;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("SewUtils.SewFeature       " + ex.Message);
                return null;
            }
            finally
            {
                sewBuilder1.Destroy();
            }
        }

        public static void SewFeatureUF(List<Body> bodys)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            List<Tag> temp = new List<Tag>();
            Tag temp1 = bodys[0].Tag;
            bodys.RemoveAt(0);
            Tag sewTag = Tag.Null;
            Tag[] disList;
            foreach (Body by in bodys)
            {
                temp.Add(by.Tag);
            }
            try
            {
                theUFSession.Modl.CreateSew(1, 1, new Tag[1] { temp1 }, bodys.Count, temp.ToArray(), 0.01, 0, out disList, out sewTag);
            }
            catch
            {

            }
        }
    }
}
