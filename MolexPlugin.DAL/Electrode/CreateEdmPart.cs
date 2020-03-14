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
    public class CreateEdmPart : ICreateAssmblePart
    {
        public EDMModel Model { get; private set; }

        public CreateEdmPart(string filePath, MoldInfoModel moldInfo)
        {
            Model = new EDMModel(filePath, moldInfo);
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
