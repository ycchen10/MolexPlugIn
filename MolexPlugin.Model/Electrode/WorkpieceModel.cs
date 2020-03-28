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
    /// 工件
    /// </summary>
    public class WorkpieceModel : AbstractModel
    {
        private string filePath;
        public WorkpieceModel(string filePath, Part part, MoldInfoModel moldInfo)
        {
            this.PartTag = part;
            this.MoldInfo = moldInfo;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.PartType = "Workpiece";
            this.filePath = filePath;
            this.GetAssembleName();
        }
        public override void CreatePart()
        {
            if (this.PartTag.Name.Equals(this.AssembleName))
                return;
            string oldPth = this.WorkpiecePath;
            this.WorkpieceDirectoryPath = filePath;
            if (!Directory.Exists(this.WorkpieceDirectoryPath))
            {
                Directory.CreateDirectory(this.WorkpieceDirectoryPath); //创建件号文件夹
            }
            this.SetAttribute();
            this.WorkpiecePath = this.WorkpieceDirectoryPath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                File.Delete(this.WorkpiecePath);
            }
            File.Move(oldPth, this.WorkpiecePath);
        }

        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + this.MoldInfo.EditionNumber;
        }

        public override void GetModelForPart(Part part)
        {
            this.GetAttribute(part);
            this.PartTag = part;
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
    }
}

