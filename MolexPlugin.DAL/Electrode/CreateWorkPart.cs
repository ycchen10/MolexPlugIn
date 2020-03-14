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
    public class CreateWorkPart : ICreateAssmblePart
    {
        public WorkModel Model { get; private set; }

        public CreateWorkPart(string filePath, MoldInfoModel moldInfo, int workNumber, Matrix4 math)
        {
            Model = new WorkModel(filePath, moldInfo, workNumber, math);
        }
        public bool CreatePart()
        {
            if (File.Exists(this.Model.WorkpiecePath))
            {
                UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "WORK" + this.Model.WorkNumber.ToString() + "已添加！");
                return false;
            }
            Model.CreatePart();
            return true;
        }
        public bool Add(ref AssembleSingleton singleton)
        {
            return singleton.AddWork(this.Model);
        }

        public Component Load(Part part)
        {
            return this.Model.Load(part);
        }
    }
}
