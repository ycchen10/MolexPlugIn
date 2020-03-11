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
    /// <summary>
    /// EDM
    /// </summary>
    public class EDMAssembleModel : AbstractAssembleModel
    {
        public EDMAssembleModel()
        {
            this.PartType = "EDM";
        }
        public EDMAssembleModel(MoldInfoModel moldInfo)
        {
            this.MoldInfo = moldInfo;
            this.PartType = "EDM";
        }
      
        public override void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber + "-EDM";
        }

    }
}
