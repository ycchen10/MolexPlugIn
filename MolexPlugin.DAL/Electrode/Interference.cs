using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极过切检查
    /// </summary>
    public class Interference
    {
        private WorkModel work;
        private ElectrodeModel eleModel;
        private List<Part> workpiecePart;
        private Part workpiece;
        private Part workPart;
        private UFSession theUFSession;
        public Interference(ElectrodeModel ele, WorkModel work, List<Part> parts)
        {
            workPart = Session.GetSession().Parts.Work;
            theUFSession = UFSession.GetUFSession();
            this.eleModel = ele;
            this.work = work;
            this.workpiecePart = parts;
            string workpieceName = ele.MoldInfo.MoldNumber + "-" + ele.MoldInfo.WorkpieceNumber + ele.MoldInfo.EditionNumber;
            foreach (Part part in workpiecePart)
            {
                if (part.Name.Equals(workpieceName))
                {
                    workpiece = part;
                }
            }
        }
        /// <summary>
        /// 移动电极
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool MoveElePart(int dir)
        {
            if ((this.eleModel.EleInfo.PitchXNum > 1 && Math.Abs(this.eleModel.EleInfo.PitchX) > 0) ||
                (this.eleModel.EleInfo.PitchYNum > 1 && Math.Abs(this.eleModel.EleInfo.PitchY) > 0))
            {
                Vector3d vec = new Vector3d();
                vec.X = (this.eleModel.EleInfo.PitchXNum - 1) * this.eleModel.EleInfo.PitchX * dir;
                vec.Y = (this.eleModel.EleInfo.PitchYNum - 1) * this.eleModel.EleInfo.PitchY * dir;
                AssmbliesUtils.MoveCompPart(this.eleModel.GetPartComp(workPart), vec, this.work.WorkMatr);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断设定值是否正确
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        private bool IsSetValueOk()
        {
            Point pt = null;
            foreach (Point k in this.eleModel.PartTag.Points.ToArray())
            {
                if (k.Name.Equals(("SetValuePoint").ToUpper()))
                    pt = k;
            }
            if (pt == null)
                return false;
            Tag ptOccsTag = theUFSession.Assem.FindOccurrence(this.eleModel.GetPartComp(workPart).Tag, pt.Tag);
            Point ptOcc = NXObjectManager.Get(ptOccsTag) as Point;
            Point3d value = ptOcc.Coordinates;
            Point3d eleSet = new Point3d(this.eleModel.EleInfo.EleSetValue[0], this.eleModel.EleInfo.EleSetValue[1], this.eleModel.EleInfo.EleSetValue[2]);
            this.work.WorkMatr.ApplyPos(ref value);
            if (UMathUtils.IsEqual(value, eleSet))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取occ下Part所有体
        /// </summary>
        /// <returns></returns>
        private List<Body> GetOccsInBods(Part pt)
        {
            List<Body> bodys = new List<Body>();
            Tag[] workpiecePartOccsTag;
            theUFSession.Assem.AskOccsOfPart(workPart.Tag, pt.Tag, out workpiecePartOccsTag);
            foreach (Body body in pt.Bodies)
            {
                Tag bodyOccsTag = theUFSession.Assem.FindOccurrence(workpiecePartOccsTag[0], body.Tag);
                bodys.Add(NXObjectManager.Get(bodyOccsTag) as Body);
            }

            return bodys;
        }

        private List<string> EleInterference(string Pich)
        {
            List<string> info = new List<string>();
            Body eleBody = GetOccsInBods(this.eleModel.PartTag)[0];
            foreach (Part work in this.workpiecePart)
            {
                List<Body> bodys = new List<Body>();
                Body workpieceBody = GetOccsInBods(work)[0];
                NXOpen.GeometricAnalysis.SimpleInterference.Result re = AnalysisUtils.SetInterferenceOutResult(eleBody, workpieceBody, out bodys);
                if (re == NXOpen.GeometricAnalysis.SimpleInterference.Result.NoInterference)
                    info.Add(this.eleModel.AssembleName + "                     " + work.Name + Pich + "没有干涉！");
                if (re == NXOpen.GeometricAnalysis.SimpleInterference.Result.InterferenceExists)
                    info.Add(this.eleModel.AssembleName + "                     " + work.Name + Pich + "有干涉！");
                if (bodys.Count > 0)
                {
                    foreach (Body body in bodys)
                    {
                        body.Layer = 252;
                    }
                }
            }
            return info;
        }
        /// <summary>
        /// 干涉体
        /// </summary>
        /// <returns></returns>
        public List<string> InterferenceOfBody()
        {
            List<string> info = new List<string>();
            if (!IsSetValueOk())
            {
                info.Add(this.eleModel.AssembleName + "                        设定值错误！");
            }
            List<string> eleStr = EleInterference("");
            if (eleStr.Count > 0)
            {
                info.AddRange(eleStr);
            }
            if (MoveElePart(-1))
            {
                List<string> moveName = EleInterference("PICH");
                info.AddRange(moveName);
                MoveElePart(1);
            }
            return info;
        }
        /// <summary>
        /// 干涉面
        /// </summary>
        public void GetInterferenceOfFace()
        {
            List<Tag> outFace = new List<Tag>();
            Body eleBody = GetOccsInBods(this.eleModel.PartTag)[0];
            Body workpieceBody = GetOccsInBods(this.workpiece)[0];
            List<Face> faces = AnalysisUtils.SetInterferenceOutFace(eleBody, workpieceBody);
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

                    Tag bodyTag1 = Intersect(bodys1[0], bodys2[0]);
                    if (bodyTag1 != Tag.Null)
                        outFace.Add(bodyTag1);
                }
            }
            SewSolidBody(outFace);

        }

        public void GetInterferenceOfArea()
        {
            Body eleBody = GetOccsInBods(this.eleModel.PartTag)[0];
            Body workpieceBody = GetOccsInBods(this.workpiece)[0];
            List<Face> faces = AnalysisUtils.SetInterferenceOutFace(eleBody, workpieceBody);
            double minArea = 0;
            for (int i = 0; i < (faces.Count) / 2 - 1; i++)
            {
                FaceData data1 = FaceUtils.AskFaceData(faces[i * 2]);
                FaceData data2 = FaceUtils.AskFaceData(faces[i * 2 + 1]);
                if (data1.Equals(data2))
                {
                    double area1 = FaceUtils.GetFaceArea(faces[i * 2]);
                    double area2 = FaceUtils.GetFaceArea(faces[i * 2 + 1]);
                    if (area1 > area2)
                        minArea += area2;
                    else
                        minArea += area1;
                }
            }
            AttributeUtils.AttributeOperation("Area", minArea, this.eleModel.PartTag);
        }
        /// <summary>
        /// 缝合面
        /// </summary>
        /// <param name="temp"></param>
        private void SewSolidBody(List<Tag> temp)
        {
            Tag temp1 = temp[0];
            temp.RemoveAt(0);
            Tag sewTag = Tag.Null;
            Tag[] disList;
            try
            {
                theUFSession.Modl.CreateSew(1, 1, new Tag[1] { temp1 }, temp.Count, temp.ToArray(), 0.01, 0, out disList, out sewTag);
            }
            catch
            {

            }

        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <returns></returns>
        private Tag Intersect(Body body1, Body body2)
        {
            Tag face3;
            Tag bodyTag1 = Tag.Null;
            try
            {
                theUFSession.Modl.IntersectBodiesWithRetainedOptions(body1.Tag, body2.Tag, false, false, out face3);
                theUFSession.Modl.AskFeatBody(face3, out bodyTag1);
                return bodyTag1;
            }
            catch
            {
                return bodyTag1;
            }
        }
    }
}
