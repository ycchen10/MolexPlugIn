using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;

namespace MolexPlugin
{
    public class test
    {

        public static void ces()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            //Body body1 = NXObjectManager.Get((Tag)75937) as Body;
            //Body body2 = NXObjectManager.Get((Tag)77911) as Body;
            ////AnalysisUtils.SetInterference(body1, body2);
            // Tag faceTag1 = (Tag)79053;
            Body body1 = NXObjectManager.Get((Tag)61221) as Body;
            Body body2 = NXObjectManager.Get((Tag)119406) as Body;
            //Tag face2 = (Tag)61860;
            //Tag face1 = (Tag)61859;
            //Tag face3;
            //theUFSession.Modl.IntersectBodiesWithRetainedOptions(face1, face2, false, false, out face3);

            // AnalysisUtils.SetInterference(body1, body2);
            //double[] ptOnObj1 = new double[3];
            //double[] ptOnObj2 = new double[3];
            //AnalysisUtils.AskMinimumDist(face1, face2, out ptOnObj1, out ptOnObj2);
            //FaceData data1 = FaceUtils.AskFaceData(face1);
            //FaceData data2 = FaceUtils.AskFaceData(face2);
            //bool ok = data1.Equals(data2);

            List<Face> faces = AnalysisUtils.SetInterferenceOutFace(body1, body2);
            for (int i = 0; i < (faces.Count) / 2 - 1; i++)
            {
                FaceData data1 = FaceUtils.AskFaceData(faces[i * 2]);
                FaceData data2 = FaceUtils.AskFaceData(faces[i * 2 + 1]);
                if (data1.Equals(data2))
                {
                    Tag face3;
                    NXOpen.Features.Feature feat1 = AssmbliesUtils.WaveFace(faces[i * 2]);
                    NXOpen.Features.Feature feat2 = AssmbliesUtils.WaveFace(faces[i * 2 + 1]);
                    Body[] bodys1 = (feat1 as NXOpen.Features.BodyFeature).GetBodies();
                    Body[] bodys2 = (feat2 as NXOpen.Features.BodyFeature).GetBodies();
                    theUFSession.Modl.IntersectBodiesWithRetainedOptions(faces[i * 2].Tag, faces[i * 2 + 1].Tag, false, false, out face3);
                }
            }
        }


    }
}
