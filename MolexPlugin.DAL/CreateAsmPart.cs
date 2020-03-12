using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using MolexPlugin.Model;
using NXOpen.Assemblies;

namespace MolexPlugin.DAL
{
    public class CreateAsmPart : ICreateAssmblePart
    {
        public ASMModel Model { get; private set; }

        public CreateAsmPart(string filePath, MoldInfoModel moldInfo)
        {
            Model = new ASMModel(filePath, moldInfo);
        }
        public bool CreatePart()
        {
            if (File.Exists(this.Model.WorkpiecePath))
                File.Delete(this.Model.WorkpiecePath);
            Model.CreatePart();
            return true;
        }

        public Component Load(Part part)
        {
            return this.Model.Load(part);
        }
    }
}
