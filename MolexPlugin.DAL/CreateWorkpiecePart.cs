using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using NXOpen.Assemblies;

namespace MolexPlugin.DAL
{
    public class CreateWorkpiecePart : ICreateAssmblePart
    {
        public WorkpieceModel Model { get; private set; }

        public CreateWorkpiecePart(string filePath,Part part, MoldInfoModel moldInfo)
        {
            Model = new WorkpieceModel(filePath,part,moldInfo);
        }
        public bool CreatePart()
        {               
            Model.CreatePart();
            return true;
        }
       
        public Component Load(Part part)
        {
            return this.Model.Load(part);
        }
    }
}
