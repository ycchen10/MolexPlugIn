using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using System.IO;
using NXOpen.UF;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 修改所有装配档
    /// </summary>
    public class ReplaceAssmblies
    {
        private AssembleModel assm;
        private bool isBorrow;
        private MoldInfoModel info;
        private Part oldCt = null;
        private Part newCt = null;
        public ReplaceAssmblies(AssembleModel assm, MoldInfoModel info, bool isBorrow)
        {
            this.assm = assm;
            this.isBorrow = isBorrow;
            this.info = info;
            GetWorkpiece();

        }
        /// <summary>
        /// 获取新旧工件
        /// </summary>
        private void GetWorkpiece()
        {
            Part workPart = Session.GetSession().Parts.Work;
            string oldWorkpieceName = assm.Asm.MoldInfo.MoldNumber + "-" + assm.Asm.MoldInfo.WorkpieceNumber + "-" + assm.Asm.MoldInfo.EditionNumber;
            string newWorkpieceName = info.MoldNumber + "-" + info.WorkpieceNumber + "-" + info.EditionNumber;
            foreach (Part pt in assm.Workpieces)
            {
                if (pt.Name.Replace("-", "").Equals(oldWorkpieceName.Replace("-", ""), StringComparison.CurrentCultureIgnoreCase))
                {
                    this.oldCt = pt;
                }
                if (pt.Name.Replace("-", "").Equals(newWorkpieceName.Replace("-", ""), StringComparison.CurrentCultureIgnoreCase))
                {
                    this.newCt = pt;
                }
            }

        }
        /// <summary>
        /// 更新图纸
        /// </summary>
        /// <returns></returns>
        private bool UpdeteDrawing(Part part)
        {
            Part workPart = Session.GetSession().Parts.Work;

            if (oldCt != null && newCt != null)
            {
                PartUtils.SetPartDisplay(part);
                foreach (NXOpen.Drawings.DrawingSheet ds in part.DrawingSheets)
                {
                    ds.Open();
                    foreach (NXOpen.Drawings.DraftingView dv in ds.GetDraftingViews())
                    {

                        Basic.DrawingUtils.HideComponent(dv, AssmbliesUtils.GetPartComp(workPart, oldCt));
                        Basic.DrawingUtils.ShowComponent(dv, AssmbliesUtils.GetPartComp(workPart, newCt));
                        dv.Update();

                    }
                }
                PartUtils.SetPartDisplay(workPart);
                return true;
            }
            else
            {
                return false;
            }

        }
        public List<string> Replace()
        {
            List<string> err = new List<string>();
            Part part;
            bool anyPartsModified;
            PartSaveStatus saveStatus;
            Session theSession = Session.GetSession();
            if (oldCt != null && newCt != null && oldCt.Equals(newCt))
                return err;
            //  AllUpdeteDrawing();
            if ((part = ReplacePart.Replace(assm.Edm, info)) != null)
            {
                info.SetAttribute(part);
                theSession.Parts.SaveAll(out anyPartsModified, out saveStatus);
                err.Add("修改" + assm.Edm.AssembleName + "成功！");

            }
            foreach (ElectrodeModel em in assm.Electrodes)
            {
                if ((part = ReplacePart.Replace(em, info)) != null)
                {
                    info.SetAttribute(part);
                    ElectrodeInfo eleInfo = new ElectrodeInfo();
                    eleInfo.GetAttribute(part);
                    string oldEleName = eleInfo.EleName;
                    string olddrwPath = em.WorkpieceDirectoryPath + oldEleName + "_dwg.prt";
                    string newEleName = oldEleName.Replace(em.MoldInfo.MoldNumber + "-" + em.MoldInfo.WorkpieceNumber, info.MoldNumber + "-" + info.WorkpieceNumber);
                    eleInfo.EleName = newEleName;
                    if (isBorrow)
                    {
                        eleInfo.BorrowName = oldEleName;
                    }
                    eleInfo.SetAttribute(part);
                    theSession.Parts.SaveAll(out anyPartsModified, out saveStatus);
                    err.Add("修改" + em.AssembleName + "成功！");
                }
            }
            foreach (WorkModel wm in assm.Works)
            {
                if ((part = ReplacePart.Replace(wm, info)) != null)
                {
                    info.SetAttribute(part);
                    theSession.Parts.SaveAll(out anyPartsModified, out saveStatus);
                    err.Add("修改" + wm.AssembleName + "成功！");
                }               
            }
            foreach (ElectrodeModel em in assm.Electrodes)
            {
                Part workPart = Session.GetSession().Parts.Work;
                ElectrodeInfo eleInfo = em.EleInfo;
                string oldEleName = eleInfo.EleName;
                string olddrwPath = em.WorkpieceDirectoryPath + oldEleName + "_dwg.prt";
                if (File.Exists(olddrwPath))
                {
                    string newEleName = oldEleName.Replace(em.MoldInfo.MoldNumber + "-" + em.MoldInfo.WorkpieceNumber, info.MoldNumber + "-" + info.WorkpieceNumber);
                    eleInfo.EleName = newEleName;
                    if (isBorrow)
                    {
                        eleInfo.BorrowName = oldEleName;
                    }
                    if (File.Exists(olddrwPath.Replace(oldEleName, newEleName)))
                    {
                        File.Delete(olddrwPath.Replace(oldEleName, newEleName));
                    }
                    File.Move(olddrwPath, olddrwPath.Replace(oldEleName, newEleName));
                    NXOpen.PartLoadStatus partLoadStatus1;
                    BasePart part1 = Session.GetSession().Parts.OpenBase(olddrwPath.Replace(oldEleName, newEleName), out partLoadStatus1);
                    PartUtils.SetPartDisplay(part1 as Part);
                    eleInfo.SetAttribute(part1 as Part);
                    NXOpen.Assemblies.Component comp = part1.ComponentAssembly.RootComponent.GetChildren()[0];
                    string workName = comp.Name.Replace(em.MoldInfo.MoldNumber + "-" + em.MoldInfo.WorkpieceNumber, info.MoldNumber + "-" + info.WorkpieceNumber);
                    Basic.AssmbliesUtils.ReplaceComp(comp, em.WorkpieceDirectoryPath + workName + ".prt", workName);
                    theSession.Parts.SaveAll(out anyPartsModified, out saveStatus);
                    PartUtils.SetPartDisplay(workPart);
                    err.Add("修改" + em.AssembleName + "_dwg" + "成功！");
                }
            }
            return err;
        }

        public void AllUpdeteDrawing()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (WorkModel wm in assm.Works)
            {
                UpdeteDrawing(wm.PartTag);
            }
            foreach (ElectrodeModel em in assm.Electrodes)
            {
                Tag partTag;
                UFPart.LoadStatus error_status;
                string olddrwPath = em.WorkpieceDirectoryPath + em.AssembleName + "_dwg.prt";
                theUFSession.Part.Open(olddrwPath, out partTag, out error_status);
                theUFSession.Part.Close(partTag, 0, 1);
                UpdeteDrawing(em.PartTag);
            }
        }
    }
}
