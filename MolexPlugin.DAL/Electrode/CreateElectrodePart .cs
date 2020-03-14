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
    public class CreateElectrodePart
    {
        public ElectrodeModel Model { get; private set; }

        public CreateElectrodePart(string filePath, int workNum, ElectrodeInfo info, MoldInfoModel moldInfo, Matrix4 mat)
        {
            Model = new ElectrodeModel(filePath, workNum, info, moldInfo, mat);
        }

        public bool Add(ref AssembleSingleton singleton)
        {
            return singleton.AddElectrode(this.Model);
        }
        /// <summary>
        /// 创建装配
        /// </summary>
        /// <returns></returns>
        public Component Create()
        {
            return Model.CreateCompPart();
        }
    }
}
