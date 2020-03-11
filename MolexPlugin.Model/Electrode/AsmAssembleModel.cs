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
    /// ASM
    /// </summary>
    public class AsmAssembleModel : AbstractAssembleModel
    {
        public AsmAssembleModel()
        {
            this.PartType = "Asm";
        }
        public AsmAssembleModel(MoldInfoModel moldInfo)
        {
            this.MoldInfo = moldInfo;
            this.PartType = "Asm";
        }

        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + "-ASM";
        }
        public override Component Load(Part part)
        {
            return null;
        }

    }
}
