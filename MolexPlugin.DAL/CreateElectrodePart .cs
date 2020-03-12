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
    public class CreateElectrodePart : ICreateAssmblePart
    {
        public ElectrodeModel Model { get; private set; }

        public CreateElectrodePart(string filePath, int workNum, ElectrodeInfo info, MoldInfoModel moldInfo, Matrix4 mat, Point3d center)
        {
            Model = new ElectrodeModel(filePath, workNum, info, moldInfo, mat, center);
        }
        public bool CreatePart()
        {          
            return true;
        }
        public bool Add(ref AssembleSingleton singleton)
        {
            return singleton.AddElectrode(this.Model);
        }
        public Component Load(Part part)
        {
            return Model.CreateCompPart(); 
        }
    }
}
