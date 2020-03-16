using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace Basic
{
    public class AnalysisUtils
    {
        /// <summary>
        /// 干涉体
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="bodys"></param>
        /// <returns></returns>
        public static NXOpen.GeometricAnalysis.SimpleInterference.Result SetInterferenceOutResult(Body body1, Body body2, out List<Body> bodys)
        {
            bodys = new List<Body>();
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.GeometricAnalysis.SimpleInterference simpleInterference1;
            simpleInterference1 = workPart.AnalysisManager.CreateSimpleInterferenceObject();
            simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferenceSolid;
            simpleInterference1.FaceInterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.FaceInterferenceMethod.AllPairs;
            simpleInterference1.FirstBody.Value = body1;
            simpleInterference1.SecondBody.Value = body2;
            NXOpen.GeometricAnalysis.SimpleInterference.Result result1;
            result1 = simpleInterference1.PerformCheck();
            NXObject[] objs = simpleInterference1.GetInterferenceResults();
            foreach (NXObject obj in objs)
            {
                bodys.Add(obj as Body);
            }
            NXOpen.NXObject nXObject1;
            nXObject1 = simpleInterference1.Commit();
            simpleInterference1.Destroy();
            return result1;
        }
        /// <summary>
        /// 干涉面
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <returns></returns>
        public static List<Face> SetInterferenceOutFace(Body body1, Body body2)
        {
            List<Face> faces = new List<Face>();
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.GeometricAnalysis.SimpleInterference simpleInterference1;
            simpleInterference1 = workPart.AnalysisManager.CreateSimpleInterferenceObject();
            simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferingFaces;
            simpleInterference1.FaceInterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.FaceInterferenceMethod.AllPairs;
            simpleInterference1.FirstBody.Value = body1;
            simpleInterference1.SecondBody.Value = body2;
            NXOpen.GeometricAnalysis.SimpleInterference.Result result1;
            NXObject[] objs = simpleInterference1.GetInterferenceResults();
            foreach (NXObject obj in objs)
            {
                faces.Add(obj as Face);
            }
            simpleInterference1.Destroy();
            return faces;
        }

    }
}
