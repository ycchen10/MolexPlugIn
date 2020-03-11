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
        public static NXOpen.GeometricAnalysis.SimpleInterference.Result SetInterference(Body body1, Body body2)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.GeometricAnalysis.SimpleInterference simpleInterference1;
            simpleInterference1 = workPart.AnalysisManager.CreateSimpleInterferenceObject();
            simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferingFaces;
            simpleInterference1.FaceInterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.FaceInterferenceMethod.AllPairs;

            simpleInterference1.FirstBody.Value = body1;
            simpleInterference1.SecondBody.Value = body2;
            NXOpen.GeometricAnalysis.SimpleInterference.Result result1;
            result1 = simpleInterference1.PerformCheck();
            NXObject[] objs = simpleInterference1.GetInterferenceResults();
            for (int i = 0; i < objs.Length / 2 - 1; i++)
            {
                LogMgr.WriteLog(objs[i * 2].Tag.ToString() + "***********" + objs[i * 2 + 1].Tag.ToString());
            }
            NXOpen.NXObject nXObject1;
            nXObject1 = simpleInterference1.Commit();
            simpleInterference1.Destroy();
            return result1;
        }
    }
}
