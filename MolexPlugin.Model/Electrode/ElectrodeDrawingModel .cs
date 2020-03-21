using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.UF;
using Basic;
using NXOpen.Assemblies;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极
    /// </summary>
    public class ElectrodeDrawingModel : AbstractModel
    {
        ///
        public WorkModel Work { get; private set; }
        /// <summary>
        /// 电极信息
        /// </summary>
        public ElectrodeInfo EleInfo { get; set; } = new ElectrodeInfo();

        public List<ElectrodeModel> Eles { get; private set; }
        public ElectrodeDrawingModel()
        {

        }
        public ElectrodeDrawingModel(WorkModel work, List<ElectrodeModel> eles)
        {
            this.Work = work;
            this.Eles = eles;
            this.PartType = "Drawing";
            this.EleInfo = eles[0].EleInfo;
            this.MoldInfo = work.MoldInfo;
            GetAssembleName();
            this.WorkpieceDirectoryPath = work.WorkpieceDirectoryPath;
            this.WorkpiecePath = work.WorkpieceDirectoryPath + this.AssembleName + ".prt";
        }
        public override void CreatePart()
        {
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
        }

        public override void GetAssembleName()
        {
            this.AssembleName = EleInfo.EleName + "_dwg";
        }

        public override void GetModelForPart(Part part)
        {
            this.GetAttribute(part);
            this.AssembleName = part.Name;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.AssembleName = Path.GetFileNameWithoutExtension(this.WorkpiecePath);
        }

        public override Component Load(Part work)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            return Basic.AssmbliesUtils.PartLoad(work, this.Work.WorkpiecePath, this.Work.AssembleName, matr, new Point3d(0, 0, 0));
        }
        protected override void GetAttribute(Part part)
        {
            base.GetAttribute(part);
            EleInfo.GetAttribute(part);
        }

        protected override void SetAttribute()
        {
            base.SetAttribute();
            EleInfo.SetAttribute(this.PartTag);
            string temp = EleInfo.Preparation[0].ToString() + "*" + EleInfo.Preparation[1].ToString() + "*" + EleInfo.Preparation[2].ToString();
            AttributeUtils.AttributeOperation("StrPre", temp, this.PartTag);
        }
        /// <summary>
        /// 获取Comp下设定点
        /// </summary>
        /// <returns></returns>
        public List<Point> GetEleCompPint()
        {
            List<Point> eleCompPoints = new List<Point>();
            foreach (ElectrodeModel ele in Eles)
            {
                Point elePt = null;
                foreach (Point pt in ele.PartTag.Points)
                {
                    if (pt.Name.ToUpper().Equals("SetValuePoint".ToUpper()))
                        elePt = pt;
                }
                if (elePt != null)
                {
                    NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(this.PartTag, ele.PartTag);
                    eleCompPoints.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, elePt.Tag) as Point);
                }
            }
            return eleCompPoints;
        }
        /// <summary>
        /// 获取Work下原点
        /// </summary>
        /// <returns></returns>
        public Point GetWorkCompPoint()
        {
            Part workPart = Session.GetSession().Parts.Work;
            UFSession theUFSession = UFSession.GetUFSession();
            Component ct = AssmbliesUtils.GetPartComp(workPart, Work.PartTag);
            foreach (Point pt in Work.PartTag.Points)
            {
                if (pt.Name.ToUpper().Equals("CenterPoint".ToUpper()))
                {
                    return AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, pt.Tag) as Point;
                }
            }
            return null;
        }
    }
}
