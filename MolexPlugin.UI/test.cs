using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;
using NXOpen.UF;
using NXOpen.Utilities;
using MolexPlugin.Model;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class test
    {

        public static void ces()
        {
            Part workPart = Session.GetSession().Parts.Work;
            #region
            //UFSession theUFSession = UFSession.GetUFSession();
            //Body body1 = NXObjectManager.Get((Tag)75937) as Body;
            //Body body2 = NXObjectManager.Get((Tag)77911) as Body;
            ////AnalysisUtils.SetInterference(body1, body2);
            // Tag faceTag1 = (Tag)79053;
            //Body body1 = NXObjectManager.Get((Tag)61221) as Body;
            //Body body2 = NXObjectManager.Get((Tag)119406) as Body;
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

            //List<Face> faces = AnalysisUtils.SetInterferenceOutFace(body1, body2);
            //for (int i = 0; i < (faces.Count) / 2 - 1; i++)
            //{
            //    FaceData data1 = FaceUtils.AskFaceData(faces[i * 2]);
            //    FaceData data2 = FaceUtils.AskFaceData(faces[i * 2 + 1]);
            //    if (data1.Equals(data2))
            //    {
            //        Tag face3;
            //        NXOpen.Features.Feature feat1 = AssmbliesUtils.WaveFace(faces[i * 2]);
            //        NXOpen.Features.Feature feat2 = AssmbliesUtils.WaveFace(faces[i * 2 + 1]);
            //        Body[] bodys1 = (feat1 as NXOpen.Features.BodyFeature).GetBodies();
            //        Body[] bodys2 = (feat2 as NXOpen.Features.BodyFeature).GetBodies();
            //        theUFSession.Modl.IntersectBodiesWithRetainedOptions(faces[i * 2].Tag, faces[i * 2 + 1].Tag, false, false, out face3);
            //    }
            //}
            #endregion
            #region   PlanarMilling

            //NCGroup meth = (NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("CU");
            //NCGroupModel groups = new NCGroupModel()
            //{
            //    GeometryGroup = (NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("WORKPIECE"),
            //    MethodGroup = (NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("CU"),
            //    ProgramGroup = (NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("O0013"),
            //    ToolGroup = (NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("EM5"),
            //};
            //FaceLoopUtils.LoopList[] list = FaceLoopUtils.AskFaceLoops((Tag)50065);
            //List<Edge> edges = new List<Edge>();
            //foreach (Tag t in list[0].EdgeList)
            //{
            //    edges.Add(NXObjectManager.Get(t) as Edge);
            //}
            //BoundaryModel condition = new BoundaryModel()
            //{
            //    BouudaryPt = new Point3d(0, 0, 0),
            //    Types = BoundarySet.BoundaryTypes.Closed,
            //    ToolSide = BoundarySet.ToolSideTypes.InsideOrLeft,
            //    Edges = edges
            //};
            //FaceMillingModel model = new FaceMillingModel(groups, "electrode", "TOP", condition);
            //model.Create("456");
            #endregion
            //string err = "";
            //Face face = NXObjectManager.Get((Tag)73347) as Face;
            //foreach (Edge edge in face.GetEdges())
            //{
            //    UFEval.Line lineData;
            //    EdgeUtils.GetLineData(edge, out lineData, ref err);
            //    LogMgr.WriteLog("Tag    " + edge.Tag);

            //    LogMgr.WriteLog("Start     " + lineData.start[0].ToString() + "          " + lineData.start[1].ToString() + "          " + lineData.start[2].ToString());
            //    LogMgr.WriteLog("End     " + lineData.end[0].ToString() + "          " + lineData.end[1].ToString() + "          " + lineData.end[2].ToString());
            //    LogMgr.WriteLog("Unit     " + lineData.unit[0].ToString() + "          " + lineData.unit[1].ToString() + "          " + lineData.unit[2].ToString());
            //}
            //foreach(Part part in Session.GetSession().Parts)
            //{
            //    Tag[] elePartOccsTag;
            //    NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            //    theUFSession.Assem.AskOccsOfPart(workPart.Tag, part.Tag, out elePartOccsTag);

            //    for(int i=0;i<elePartOccsTag.Length;i++)
            //    {
            //        string name;
            //        theUFSession.Obj.AskName(elePartOccsTag[i], out name);
            //        ClassItem.Print(name);
            //    }
            //    ClassItem.Print(elePartOccsTag.Length.ToString());

            //}

            //Part part = NXObjectManager.Get((Tag)44873) as Part;
            //AssmbliesUtils.WaveAssociativeBodys(part.Bodies.ToArray());

            //ElectrodeModel ele = new ElectrodeModel();
            //ele.GetModelForPart(workPart);
            //AnalyzeElectrode analyze = new AnalyzeElectrode(ele);
            //double min = analyze.GetMinDis();
            //AnalyzeBuilder builder = analyze.AnalyzeBody();
            //foreach(AnalyzeFaceSlopeAndRadius ar in builder.AnalyzeFaces)
            //{
            //    LogMgr.WriteLog(ar.face.Tag.ToString());
            //}
            // UserInfoSingleton.Serialize();
            //ControlValue.Serialize();

            //UFSession theUFSession = UFSession.GetUFSession();

            //List<Tag> featureTags = new List<Tag>();
            //Tag groupTag = Tag.Null;
            //foreach (NXOpen.Features.Feature fe in workPart.Features)
            //{
            //    featureTags.Add(fe.Tag);
            //}
            //theUFSession.Modl.CreateSetOfFeature("特征", featureTags.ToArray(), featureTags.Count, 1, out groupTag);

        }
    }
}
