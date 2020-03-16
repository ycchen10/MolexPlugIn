using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class InterferenceBuilder
    {
        private WorkModel work;
        private List<ElectrodeModel> eles;
        private AssembleModel assemble;
        private UFSession theUFSession;
        public InterferenceBuilder(Part part)
        {
            work = new WorkModel();
            theUFSession = UFSession.GetUFSession();
            work.GetModelForPart(part);
            string asm = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber + "-ASM";
            assemble = AssembleSingleton.Instance().GetAssemble(asm);
            eles = assemble.Electrodes.Where(a => a.WorkNumber == work.WorkNumber).ToList();
        }
        /// <summary>
        /// 获取occ下工件体
        /// </summary>
        /// <returns></returns>
        private List<Body> GetWorkpieceBods()
        {
            List<Body> bodys = new List<Body>();
            foreach (Part pt in this.assemble.Workpieces)
            {
                Tag[] workpiecePartOccsTag;
                theUFSession.Assem.AskOccsOfPart(work.PartTag.Tag, pt.Tag, out workpiecePartOccsTag);
                foreach (Body body in pt.Bodies)
                {
                    Tag bodyOccsTag = theUFSession.Assem.FindOccurrence(workpiecePartOccsTag[0], body.Tag);
                    bodys.Add(NXObjectManager.Get(bodyOccsTag) as Body);
                }
            }
            return bodys;
        }
        /// <summary>
        /// 获取occ下电极体
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        private Body GetEleBody(ElectrodeModel ele)
        {
            Tag[] elePartOccsTag;
            theUFSession.Assem.AskOccsOfPart(work.PartTag.Tag, ele.PartTag.Tag, out elePartOccsTag);
            Tag bodyOccsTag = theUFSession.Assem.FindOccurrence(elePartOccsTag[0], ele.PartTag.Bodies.ToArray()[0].Tag);
            return NXObjectManager.Get(bodyOccsTag) as Body;
        }
        private bool MoveEle(ElectrodeModel ele, int i)
        {
            if ((ele.EleInfo.PitchXNum > 1 && Math.Abs(ele.EleInfo.PitchX) > 0) ||
                (ele.EleInfo.PitchXNum > 1 && Math.Abs(ele.EleInfo.PitchY) > 0))
            {
                Vector3d vec = new Vector3d();
                vec.X = (ele.EleInfo.PitchXNum - 1) * ele.EleInfo.PitchX * i;
                vec.Y = (ele.EleInfo.PitchYNum - 1) * ele.EleInfo.PitchY * i;
                AssmbliesUtils.MoveCompPart(ele.GetPartComp(work.PartTag), vec, this.work.WorkMatr);
                return true;
            }
            return false;
        }
        private bool IsSetValueOk(ElectrodeModel ele)
        {
            Point pt = ele.PartTag.Points.ToArray()[0];
            Tag ptOccsTag = theUFSession.Assem.FindOccurrence(ele.GetPartComp(work.PartTag).Tag, pt.Tag);
            Point ptOcc = NXObjectManager.Get(ptOccsTag) as Point;
            Point3d value = ptOcc.Coordinates;
            Point3d eleSet = new Point3d(ele.EleInfo.EleSetValue[0], ele.EleInfo.EleSetValue[1], ele.EleInfo.EleSetValue[2]);
            this.work.WorkMatr.ApplyPos(ref value);
            if (UMathUtils.IsEqual(value, eleSet))
            {
                return true;
            }
            return false;
        }

        public List<string> ExamineEle()
        {
            List<string> info = new List<string>();
            foreach (ElectrodeModel ele in this.eles)
            {
                Body bodyEle = GetEleBody(ele);
                
                foreach (Body workpiece in GetWorkpieceBods())
                {
              //      NXOpen.GeometricAnalysis.SimpleInterference.Result re = SimpleInterferenceUtils.SetInterference(bodyEle, workpiece);
                }
            }
            return info;
        }

    }
}
