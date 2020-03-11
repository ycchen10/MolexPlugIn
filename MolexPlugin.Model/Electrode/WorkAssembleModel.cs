using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    public class WorkAssembleModel : AbstractAssembleModel, IComparable<WorkAssembleModel>
    {

        public int WorkNumber { get; private set; }

        public Matrix4 Matr { get; private set; }
        public WorkAssembleModel()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moldInfo"></param>
        /// <param name="workNumber"></param>
        /// <param name="matr"></param>
        public WorkAssembleModel(MoldInfoModel moldInfo, int workNumber, Matrix4 matr)
        {
            this.MoldInfo = moldInfo;
            this.WorkNumber = workNumber;
            this.Matr = matr;
            this.PartType = "Work";

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
        protected override void SetAttribute()
        {
            base.SetAttribute();
            AttributeUtils.AttributeOperation("WorkNumber", this.WorkNumber, this.PartTag);
            AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(this.Matr), this.PartTag);
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
            this.Matr = StringToMatrx4(temp);
        }
        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + "-WORK" + WorkNumber.ToString();
        }
        /// <summary>
        /// 修改矩阵
        /// </summary>
        /// <param name="matr"></param>
        public void AlterMatr(Matrix4 matr)
        {
            this.Matr = matr;
            AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(matr), this.PartTag);
        }

        public int CompareTo(WorkAssembleModel other)
        {
            return this.WorkNumber.CompareTo(other.WorkNumber);
        }
        /// <summary>
        /// 创建工件
        /// </summary>
        public override bool CreatePart(string filePath)
        {
            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                return false;
            }
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
            return true;
        }

    }
}
