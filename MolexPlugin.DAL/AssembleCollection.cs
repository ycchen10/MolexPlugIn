using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MolexPlugin.Model;
using Basic;

namespace MolexPlugin.DAL
{
    public class AssembleCollection
    {
        private AssembleModle modle = null;
        public AssembleModle Modle
        {
            get
            {
                return modle;
            }
        }

        private Session theSession;
        public AssembleCollection()
        {
            theSession = Session.GetSession();
            GetModle();
        }
        /// <summary>
        /// 获取装配属性
        /// </summary>
        private void GetModle()
        {
            Part workPart = theSession.Parts.Work;
            MoldInfoModel info = new MoldInfoModel();
            info.GetAttribute(workPart);
            string name = info.MoldNumber + "-" + info.WorkpieceNumber;
            if (this.modle == null)
            {
                this.modle = new AssembleModle();
                foreach (Part part in theSession.Parts)
                {
                    if (part.Name.Length < name.Length)
                        continue;
                    if (part.Name.Substring(0, name.Length).Equals(name))
                    {
                        string partType = AttributeUtils.GetAttrForString(part, "PartType");

                        switch (partType)
                        {
                            case "Asm":
                                {
                                    AsmAssembleModel asm = new AsmAssembleModel();
                                    asm.GetPart(part);
                                    this.modle.AsmModel = asm;
                                    break;
                                }
                            case "Edm":
                                {
                                    EDMAssembleModel edm = new EDMAssembleModel();
                                    edm.GetPart(part);
                                    this.modle.EdmModel = edm;
                                    break;
                                }
                            case "Work":
                                {
                                    WorkAssembleModel model = new WorkAssembleModel();
                                    model.GetPart(part);
                                    this.modle.WorkModel.Add(model);
                                    break;
                                }
                            case "Electrode":
                                {
                                    ElectrodeAssembleModel model = new ElectrodeAssembleModel();
                                    model.GetPart(part);
                                    this.modle.EleModel.Add(model);
                                    break;
                                }
                            default:
                                break;

                        }

                    }
                }
            }


        }
        /// <summary>
        /// 添加Work
        /// </summary>
        /// <param name="work"></param>
        public void AddWork(WorkAssembleModel work)
        {
            if (!this.modle.WorkModel.Exists(x => x.AssembleName == work.AssembleName))//判断work里面是否有
            {
                this.modle.WorkModel.Add(work);
            }
            else
            {
                WorkAssembleModel tem = this.modle.WorkModel.Find(x => x.AssembleName.Contains(work.AssembleName));
                this.modle.WorkModel.Remove(tem);
            }
        }
        /// <summary>
        /// 添加电极
        /// </summary>
        /// <param name="ele"></param>
        public void AddEle(ElectrodeAssembleModel ele)
        {
            if (!this.modle.EleModel.Exists(x => x.AssembleName == ele.AssembleName))//判断work里面是否有
            {
                this.modle.EleModel.Add(ele);
            }
            else
            {
                ElectrodeAssembleModel tem = this.modle.EleModel.Find(x => x.AssembleName.Contains(ele.AssembleName));
                this.modle.EleModel.Remove(ele);
            }
        }
    }
}
