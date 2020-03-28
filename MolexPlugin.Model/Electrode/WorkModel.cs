using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;
using NXOpen.Assemblies;
using NXOpen.UF;

namespace MolexPlugin.Model
{
    /// <summary>
    /// work
    /// </summary>
    public class WorkModel : AbstractModel, IComparable<WorkModel>
    {
        /// <summary>
        /// work号
        /// </summary>
        public int WorkNumber { get; private set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 WorkMatr { get; private set; }
        public WorkModel()
        {

        }
        public WorkModel(string filePath, MoldInfoModel moldInfo, int workNumber, Matrix4 matr)
        {
            this.MoldInfo = moldInfo;
            this.WorkNumber = workNumber;
            this.WorkMatr = matr;
            this.PartType = "Work";
            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
        }

        public override void CreatePart()
        {
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
        }

        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + "-WORK" + WorkNumber.ToString();
        }

        public override void GetModelForPart(Part part)
        {
            this.GetAttribute(part);
            this.AssembleName = part.Name;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.AssembleName = Path.GetFileNameWithoutExtension(this.WorkpiecePath);
        }

        public override Component Load(Part parentPart)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            return Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, matr, new Point3d(0, 0, 0));
        }
        protected override void SetAttribute()
        {
            base.SetAttribute();
            AttributeUtils.AttributeOperation("WorkNumber", this.WorkNumber, this.PartTag);
            AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(this.WorkMatr), this.PartTag);
        }
        protected override void GetAttribute(Part part)
        {
            base.GetAttribute(part);
            this.WorkNumber = AttributeUtils.GetAttrForInt(part, "WorkNumber");
            string[] temp = new string[4];
            for (int i = 0; i < 4; i++)
            {
                temp[i] = AttributeUtils.GetAttrForString(part, "Matrx4", i);
            }
            this.WorkMatr = StringToMatrx4(temp);
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

        /// <summary>
        /// 修改矩阵
        /// </summary>
        /// <param name="matr"></param>
        public void AlterMatr(Matrix4 matr)
        {
            this.WorkMatr = matr;
            AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(matr), this.PartTag);
        }
        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(WorkModel other)
        {
            return this.WorkNumber.CompareTo(other.WorkNumber);
        }

        /// <summary>
        /// 画中心点和中心线
        /// </summary>
        /// <param name="disPt"></param>
        public void CreatePointAndCenterLine(Point3d centerPt, Point3d disPt)
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (workPart.Tag != PartTag.Tag)
            {
                NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(workPart, PartTag);
                PartUtils.SetPartWork(ct);
            }
            CreateCenterLine(centerPt, disPt);
            CreateCenterPoint();
            PartUtils.SetPartWork(null);
        }

        /// <summary>
        /// 设置中心线
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="line"></param>
        private void SetLineObj(int layer, params Line[] line)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (Line temp in line)
            {
                theUFSession.Obj.SetColor(temp.Tag, 186);
                theUFSession.Obj.SetLayer(temp.Tag, layer);
                theUFSession.Obj.SetFont(temp.Tag, 7);
            }

        }
        private void SetOrigin(int layer, params Point[] pt)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (Point i in pt)
            {
                theUFSession.Obj.SetColor(i.Tag, 186);
                theUFSession.Obj.SetLayer(i.Tag, 20);
                theUFSession.Obj.SetName(i.Tag, "CenterPoint");
            }
        }
        /// <summary>
        /// 创建中心线
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="disPt"></param>
        private void CreateCenterLine(Point3d centerPt, Point3d disPt)
        {

            foreach (Line line in this.PartTag.Lines)
            {
                if (line.Color == 186 && line.Layer == 20)
                    return;
            }
            Point3d minX = new Point3d(centerPt.X - disPt.X - 5.0, 0, 0);
            Point3d maxX = new Point3d(centerPt.X + disPt.X + 5.0, 0, 0);
            Point3d minY = new Point3d(0, centerPt.Y - disPt.Y - 5.0, 0);
            Point3d maxY = new Point3d(0, centerPt.Y + disPt.Y + 5.0, 0);

            Matrix4 inver = this.WorkMatr.GetInversMatrix();
            inver.ApplyPos(ref minX);
            inver.ApplyPos(ref maxX);
            inver.ApplyPos(ref minY);
            inver.ApplyPos(ref maxY);
            Line line1 = this.PartTag.Curves.CreateLine(minX, maxX);
            Line line2 = this.PartTag.Curves.CreateLine(minY, maxY);
            SetLineObj(20, new Line[2] { line1, line2 });
        }
        /// <summary>
        /// 创建中心点
        /// </summary>
        public Point CreateCenterPoint()
        {
            foreach (Point pt in PartTag.Points)
            {
                if (pt.Name.ToUpper().Equals("CenterPoint".ToUpper()))
                {
                    return pt;
                }
            }
            Point3d temp = new Point3d(0, 0, 0);
            Matrix4 inver = this.WorkMatr.GetInversMatrix();
            inver.ApplyPos(ref temp);
            Point originPoint = PointUtils.CreatePoint(temp);
            SetOrigin(20, originPoint);
            return originPoint;
        }
        /// <summary>
        /// 设置连接体
        /// </summary>
        public void WaveBodys()
        {
            Part workPart = Session.GetSession().Parts.Work;
            UFSession theUFSession = UFSession.GetUFSession();
            if (workPart.Tag != this.PartTag.Tag)
            {
                NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(workPart, this.PartTag);
                PartUtils.SetPartWork(ct);
            }
            foreach (Part part in Session.GetSession().Parts)
            {
                string type = AttributeUtils.GetAttrForString(part, "PartType");
                if (type.Equals("Workpiece"))
                {
                    Body[] bodys = part.Bodies.ToArray();
                    NXOpen.Features.Feature feat = AssmbliesUtils.WaveAssociativeBodys(bodys);
                    Body[] waveBodys = ((NXOpen.Features.BodyFeature)feat).GetBodies();
                    foreach (Body body in waveBodys)
                    {
                        body.Layer = 2;
                        theUFSession.Layer.SetStatus(2, 2);
                    }
                    break;
                }
            } 
            PartUtils.SetPartWork(null); 
        }
    }
}
