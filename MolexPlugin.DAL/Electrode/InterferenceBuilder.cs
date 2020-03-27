using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class InterferenceBuilder
    {
        private WorkModel work;
        private List<ElectrodeModel> eles;
        private AssembleModel assemble;
        private UFSession theUFSession;
        public InterferenceBuilder(Part part)
        {
            work = new WorkModel();
            theUFSession = UFSession.GetUFSession();
            work.GetModelForPart(part);
            string asm = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber;
            assemble = AssembleSingleton.Instance().GetAssemble(asm);
            eles = assemble.Electrodes.Where(a => a.WorkNumber == work.WorkNumber).ToList();
        }


        public bool CreateInterferenceBody()
        {
            List<string> info = new List<string>();
            foreach (ElectrodeModel ele in eles)
            {
                Interference eleInter = new Interference(ele, this.work, this.assemble.Workpieces);
                List<string> temp = eleInter.InterferenceOfBody();
                info.AddRange(temp);
            }
            if (info.Count > 0)
            {
                foreach (string str in info)
                {
                    ClassItem.Print(str);
                }
                return false;
            }          
            return true;
        }

        public void CreateInterferenceFace()
        {
            foreach (ElectrodeModel ele in eles)
            {
                Interference eleInter = new Interference(ele, this.work, this.assemble.Workpieces);
                eleInter.GetInterferenceOfArea();
            }
        }

    }
}
