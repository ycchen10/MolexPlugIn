using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;
using NXOpen.Assemblies;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极
    /// </summary>
    public class ElectrodeAssembleModel : AbstractAssembleModel, IComparable<ElectrodeAssembleModel>
    {
        /// <summary>
        /// 电极信息
        /// </summary>
        public ElectrodeInfo EleInfo { get; set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get; set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; set; }

        public ElectrodeAssembleModel()
        {
            EleInfo = new ElectrodeInfo();
        }
        public ElectrodeAssembleModel(ElectrodeInfo info, MoldInfoModel mold, Matrix4 mat, Point3d center)
        {
            this.PartType = "Electrode";
            this.EleInfo = info;
            this.MoldInfo = mold;
            this.Matr = mat;
            this.CenterPt = center;

        }
        public ElectrodeAssembleModel(ElectrodeInfo info, MoldInfoModel mold, Part part)
        {
            this.PartType = "Electrode";
            this.EleInfo = info;
            this.MoldInfo = mold;
            GetAssembleName();
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.PartTag = part;
            SetAttribute();

        }
        public override void GetAssembleName()
        {
            this.AssembleName = this.EleInfo.EleName;

        }

        protected override void GetAttribute(Part part)
        {
            base.GetAttribute(part);
            EleInfo.GetAttribute(part);

        }

        protected override void SetAttribute()
        {
            this.MoldInfo.SetAttribute(this.PartTag);
            EleInfo.SetAttribute(this.PartTag);
            AttributeUtils.AttributeOperation("PartType", this.PartType, this.PartTag);
        }

        /// <summary>
        /// 更新表达式
        /// </summary>
        public void UpdatePartForExpression()
        {

        }
        public override Component Load(Part part)
        {
            return Basic.AssmbliesUtils.PartLoad(part, this.WorkpiecePath, this.AssembleName, this.Matr, this.CenterPt);
        }

        public int CompareTo(ElectrodeAssembleModel other)
        {
            return this.EleInfo.EleNumber.CompareTo(other.EleInfo.EleNumber);
        }

        /// 创建工件
        /// </summary>
        public override bool CreatePart(string filePath)
        {
            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                ClassItem.MessageBox("电极重名", NXMessageBox.DialogType.Error);
                return false;
            }
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
            return true;
        }

        public NXOpen.Assemblies.Component CreateCompPart(string filePath)
        {

            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                ClassItem.MessageBox("电极重名", NXMessageBox.DialogType.Error);
                return null;
            }
            CsysUtils.SetWcsOfCenteAndMatr(this.CenterPt, this.Matr.GetMatrix3());
            NXObject obj = AssmbliesUtils.CreateNew(this.AssembleName, WorkpiecePath);
            NXOpen.Assemblies.Component comp = obj as NXOpen.Assemblies.Component;
            this.PartTag = obj.Prototype as Part;
            SetAttribute();
          //  CsysUtils.SetWcsToAbs();
            return comp;
        }
    }
}
