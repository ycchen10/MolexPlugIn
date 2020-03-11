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
    /// <summary>
    /// 创建组立档
    /// </summary>
    public class AddEdmAsmBuilder
    {

        public static void CreateBuilder(MoldInfoModel moldInfo)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            Part workPart = Session.GetSession().Parts.Work;


            AbstractAssembleModel workPiece = new WorkPieceAssembleModel(workPart, moldInfo);
            AbstractAssembleModel asm = new AsmAssembleModel(moldInfo);
            AbstractAssembleModel work = new WorkAssembleModel(moldInfo, 1, matr);
            AbstractAssembleModel edm = new EDMAssembleModel(moldInfo);
            string path;
            if (workPart.Name.Equals(workPiece.AssembleName))
            {
                path = workPiece.WorkpieceDirectoryPath;
            }
            else
                path = workPiece.WorkpieceDirectoryPath + moldInfo.WorkpieceNumber + "-" + moldInfo.EditionNumber + "\\";

            workPiece.CreatePart(path);

            asm.CreatePart(path);
            bool ok = work.CreatePart(path);
            edm.CreatePart(path);
            if (!ok)
                return;
            work.Load(asm.PartTag);
            edm.Load(work.PartTag);
            workPiece.Load(edm.PartTag);
            PartUtils.SetPartDisplay(asm.PartTag);
            bool anyPartsModified1;
            NXOpen.PartSaveStatus partSaveStatus1;
            Session.GetSession().Parts.SaveAll(out anyPartsModified1, out partSaveStatus1);


        }

    }
}
