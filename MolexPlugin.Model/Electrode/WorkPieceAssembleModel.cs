using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 工件
    /// </summary>
    public class WorkPieceAssembleModel : AbstractAssembleModel
    {
        public WorkPieceAssembleModel(Part part, MoldInfoModel moldInfo)
        {
            this.PartTag = part;
            this.MoldInfo = moldInfo;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            GetAssembleName();
            this.PartType = "workPiece";
            this.SetAttribute();

        }
        public WorkPieceAssembleModel()
        {

        }
        public override bool CreatePart(string filePath)
        {
            if (this.PartTag.Name.Equals(this.AssembleName))
                return false;
            string oldPth = this.WorkpiecePath;
            this.WorkpieceDirectoryPath = filePath;
            if (!Directory.Exists(this.WorkpieceDirectoryPath))
            {
                Directory.CreateDirectory(this.WorkpieceDirectoryPath); //创建件号文件夹
            }
            this.WorkpiecePath = this.WorkpieceDirectoryPath + this.AssembleName + ".prt";
            File.Move(oldPth, this.WorkpiecePath);
            return true;
        }

        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + this.MoldInfo.EditionNumber;
        }
    }
}
