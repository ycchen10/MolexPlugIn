using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;
using NXOpen.Assemblies;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极
    /// </summary>
    public class ElectrodeModel : AbstractModel, IComparable<ElectrodeModel>
    {

        public int WorkNumber { get; set; }
        /// <summary>
        /// 电极信息
        /// </summary>
        public ElectrodeInfo EleInfo { get; set; } = new ElectrodeInfo();
        /// <summary>
        /// 电极矩阵
        /// </summary>
        public Matrix4 EleMatr { get; set; }
        /// <summary>
        /// 电极中心点在work位置点(绝对点)
        /// </summary>
        public Point3d CenterPt { get; set; }

     

        public ElectrodeModel()
        {

        }
        public ElectrodeModel(string filePath, int workNum, ElectrodeInfo info, MoldInfoModel mold, Matrix4 mat)
        {
            this.PartType = "Electrode";
            this.EleInfo = info;
            this.MoldInfo = mold;
            this.EleMatr = mat;
            this.CenterPt = GetMatrCenter(mat);
            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
            this.WorkNumber = workNum;
        }
        public override void CreatePart()
        {
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
        }

        public override void GetAssembleName()
        {
            this.AssembleName = EleInfo.EleName;
        }

        public override void GetModelForPart(Part part)
        {
            this.GetAttribute(part);
            this.AssembleName = part.Name;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.AssembleName = Path.GetFileNameWithoutExtension(this.WorkpiecePath);
            this.CenterPt = GetMatrCenter(this.EleMatr);
        }

        public override Component Load(Part parentPart)
        {
            return Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, this.EleMatr, this.CenterPt);
        }
        protected override void GetAttribute(Part part)
        {
            base.GetAttribute(part);
            EleInfo.GetAttribute(part);
            string[] temp = new string[4];
            for (int i = 0; i < 4; i++)
            {
                temp[i] = AttributeUtils.GetAttrForString(part, "Matrx4", i);
            }
            this.EleMatr = StringToMatrx4(temp);
            this.WorkNumber = AttributeUtils.GetAttrForInt(part, "WorkNumber");
        }

        protected override void SetAttribute()
        {
            base.SetAttribute();
            EleInfo.SetAttribute(this.PartTag);
            AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(this.EleMatr), this.PartTag);
            AttributeUtils.AttributeOperation("WorkNumber", this.WorkNumber, this.PartTag);
        }
        /// <summary>
        /// 创建装配
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public NXOpen.Assemblies.Component CreateCompPart()
        {

            CsysUtils.SetWcsOfCenteAndMatr(this.CenterPt, this.EleMatr.GetMatrix3());
            NXObject obj = AssmbliesUtils.CreateNew(this.AssembleName, WorkpiecePath);
            NXOpen.Assemblies.Component comp = obj as NXOpen.Assemblies.Component;
            this.PartTag = obj.Prototype as Part;
            if (this.PartTag != null)
            {
                SetAttribute();
            }
            CsysUtils.SetWcsToAbs();
            return comp;
        }
        /// <summary>
        /// 获取电极矩阵
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Matrix4 GetEleMatrix4(Part part)
        {
            string[] temp = new string[4];
            for (int i = 0; i < 4; i++)
            {
                temp[i] = AttributeUtils.GetAttrForString(part, "Matrx4", i);
            }

            return StringToMatrx4(temp);
        }

        /// <summary>
        /// 矩阵转字符
        /// </summary>
        /// <param name="matr"></param>
        /// <returns></returns>
        private string[] Matrx4ToString(Matrix4 matr)
        {
            string[] temp = new string[4];

            for (int i = 0; i < 4; i++)
            {
                temp[i] = Math.Round(matr.matrix[i, 0], 4).ToString() + "," + Math.Round(matr.matrix[i, 1], 4).ToString() + "," +
                   Math.Round(matr.matrix[i, 2], 4).ToString() + "," + Math.Round(matr.matrix[i, 3], 4).ToString();
            }
            return temp;
        }
        /// <summary>
        /// 字符转矩阵
        /// </summary>
        /// <param name="matrString"></param>
        /// <returns></returns>
        private Matrix4 StringToMatrx4(string[] matrString)
        {
            double[,] temp = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                string[] ch = { "," };
                string[] str = matrString[i].Split(ch, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < 4; j++)
                {
                    temp[i, j] = Convert.ToDouble(str[j]);
                }
            }
            return new Matrix4(temp);
        }
        private Point3d GetMatrCenter(Matrix4 mat)
        {
            Matrix4 inver = mat.GetInversMatrix();
            Point3d ceneter = new Point3d(0, 0, 0);
            inver.ApplyPos(ref ceneter);
            return ceneter;
        }

        public int CompareTo(ElectrodeModel other)
        {
            return this.EleInfo.EleNumber.CompareTo(other.EleInfo.EleNumber);
        }
    }
}
