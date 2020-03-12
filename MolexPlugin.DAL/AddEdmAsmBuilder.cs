using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class AddEdmAsmBuilder
    {
        public static void CreateBuilder(MoldInfoModel moldInfo)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Assemblies.Component[] ct = workPart.ComponentAssembly.RootComponent.GetChildren();
            if (ct.Length == 0 || ct == null)
            {
                UI.GetUI().NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "工件是装配关系");
                return;
            }
            CsysUtils.SetWcsToAbs();
            Matrix4 mat = new Matrix4();
            mat.Identity();
            string name = moldInfo.MoldNumber + "-" + moldInfo.WorkpieceNumber + moldInfo.EditionNumber;
            string partfull = workPart.FullPath;
            string path;
            if (workPart.Name.Equals(name))
            {
                path = Path.GetDirectoryName(partfull) + "\\";
            }
            else
            {
                path = Path.GetDirectoryName(partfull) + "\\" + moldInfo.WorkpieceNumber + "-" + moldInfo.EditionNumber + "\\";
            }

            CreateAsmPart asm = new CreateAsmPart(path, moldInfo);
            CreateEdmPart edm = new CreateEdmPart(path, moldInfo);
            CreateWorkPart work = new CreateWorkPart(path, moldInfo, 1, mat);
            CreateWorkpiecePart workpiece = new CreateWorkpiecePart(path, workPart, moldInfo);


            workpiece.CreatePart();
            asm.CreatePart();
            edm.CreatePart();
            work.CreatePart();

            workpiece.Load(edm.Model.PartTag);
            edm.Load(work.Model.PartTag);
            work.Load(asm.Model.PartTag);

            PartUtils.SetPartDisplay(asm.Model.PartTag);
            bool anyPartsModified1;
            NXOpen.PartSaveStatus partSaveStatus1;
            Session.GetSession().Parts.SaveAll(out anyPartsModified1, out partSaveStatus1);

        }


    }
}
