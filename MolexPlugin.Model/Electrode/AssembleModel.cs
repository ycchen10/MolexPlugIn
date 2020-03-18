using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 装配实体
    /// </summary>
    public class AssembleModel
    {

        private string moldAndWorkpieceNum;
        public ASMModel Asm { get; private set; } = null;

        public List<WorkModel> Works { get; private set; } = new List<WorkModel>();

        public EDMModel Edm { get; private set; } = null;

        public List<ElectrodeModel> Electrodes { get; private set; } = new List<ElectrodeModel>();

        public List<Part> Workpieces { get; private set; } = new List<Part>();

        public AssembleModel(string moldAndWorkpieceNum)
        {
            this.moldAndWorkpieceNum = moldAndWorkpieceNum;
            GetAssembleInfo();
            GetWorkpiece();
        }
        /// <summary>
        /// 获取全部装配
        /// </summary>
        private void GetAssembleInfo()
        {
            foreach (Part pt in Session.GetSession().Parts)
            {
                if (pt.Name.Length > moldAndWorkpieceNum.Length)
                {
                    if (moldAndWorkpieceNum.Equals(pt.Name.Substring(0, moldAndWorkpieceNum.Length))) //判断是否一个模号
                    {
                        string partType = AttributeUtils.GetAttrForString(pt, "PartType");

                        switch (partType)
                        {
                            case "ASM":
                                {
                                    ASMModel asm = new ASMModel();
                                    asm.GetModelForPart(pt);
                                    this.Asm = asm;
                                    break;
                                }
                            case "EDM":
                                {
                                    EDMModel edm = new EDMModel();
                                    edm.GetModelForPart(pt);
                                    this.Edm = edm;
                                    break;
                                }
                            case "Work":
                                {
                                    WorkModel model = new WorkModel();
                                    model.GetModelForPart(pt);
                                    this.Works.Add(model);
                                    break;
                                }
                            case "Electrode":
                                {
                                    ElectrodeModel model = new ElectrodeModel();
                                    model.GetModelForPart(pt);
                                    this.Electrodes.Add(model);
                                    break;
                                }
                            default:
                                break;

                        }
                    }
                }
            }
        }

        private void GetWorkpiece()
        {
            if (this.Edm != null)
            {
                NXOpen.Assemblies.Component edmComp =this.Edm.GetPartComp(Asm.PartTag);
                if (edmComp != null)
                {
                    foreach (NXOpen.Assemblies.Component ct in edmComp.GetChildren())
                    {
                        Workpieces.Add(ct.Prototype as Part);
                    }
                }
            }
        }
        /// <summary>
        /// 添加Work
        /// </summary>
        /// <param name="work"></param>
        public void AddWork(WorkModel work)
        {
            if (!this.Works.Exists(x => x.AssembleName == work.AssembleName))
            {
                this.Works.Add(work);
            }
            else
            {
                WorkModel temp = this.Works.Find(x => x.AssembleName == work.AssembleName);
                this.Works.Remove(temp);
                this.Works.Add(work);
            }
        }

        /// <summary>
        /// 添加电极
        /// </summary>
        /// <param name="ele"></param>
        public void AddElectrode(ElectrodeModel ele)
        {
            if (!this.Electrodes.Exists(x => x.AssembleName == ele.AssembleName))
            {
                this.Electrodes.Add(ele);
            }
            else
            {
                ElectrodeModel temp = this.Electrodes.Find(x => x.AssembleName == ele.AssembleName);
                this.Electrodes.Remove(temp);
                this.Electrodes.Add(ele);
            }
        }
    }
}
