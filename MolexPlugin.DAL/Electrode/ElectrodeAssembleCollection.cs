using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodeAssembleCollection
    {
        /// <summary>
        /// 获取ASM
        /// </summary>
        /// <returns></returns>
        public static Part GetAsmCollection()
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            MoldInfoModel mold = new MoldInfoModel();
            mold.GetAttribute(workPart);
            string asmName = mold.MoldNumber + "-" + mold.WorkpieceNumber + "-ASM";
            if (workPart.Name.Equals(asmName))
                return workPart;
            try
            {
                Part asmPart = theSession.Parts.FindObject(asmName) as Part;
                return asmPart;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取Work
        /// </summary>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        public static List<Part> GetWorkCollection(Part asmPart)
        {
            List<Part> workParts = new List<Part>();
            NXOpen.Assemblies.Component[] workComps = asmPart.ComponentAssembly.RootComponent.GetChildren();
            foreach (NXOpen.Assemblies.Component ct in workComps)
            {
                workParts.Add(ct.Prototype as Part);
            }
            return workParts;
        }
        /// <summary>
        /// 获取电极
        /// </summary>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        public static List<Part> GetElectrodeCollection(Part asmPart)
        {
            List<Part> eleParts = new List<Part>();
            MoldInfoModel mold = new MoldInfoModel();
            mold.GetAttribute(asmPart);
            string edmName = mold.MoldNumber + "-" + mold.WorkpieceNumber + "-EDM";
            foreach (NXOpen.Assemblies.Component comp in asmPart.ComponentAssembly.RootComponent.GetChildren())
            {
                NXOpen.Assemblies.Component[] workChildren = comp.GetChildren();
                foreach (NXOpen.Assemblies.Component ct in workChildren)
                {
                    if (!ct.Name.Equals(edmName))
                        eleParts.Add(ct.Prototype as Part);
                }
            }
            return eleParts;
        }
        /// <summary>
        /// 获取EDM
        /// </summary>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        public static Part GetEDMCollection(Part asmPart)
        {
            MoldInfoModel mold = new MoldInfoModel();
            mold.GetAttribute(asmPart);
            string edmName = mold.MoldNumber + "-" + mold.WorkpieceNumber + "-EDM";
            foreach (NXOpen.Assemblies.Component comp in asmPart.ComponentAssembly.RootComponent.GetChildren())
            {
                NXOpen.Assemblies.Component[] workChildren = comp.GetChildren();
                foreach (NXOpen.Assemblies.Component ct in workChildren)
                {
                    if (ct.Name.Equals(edmName))
                        return ct.Prototype as Part;
                }
            }
            return null;
        }

    }
}
