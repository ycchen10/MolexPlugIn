using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class AddWorkBuilder
    {
        /// <summary>
        /// 获取Work号
        /// </summary>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        public static List<int> GetWorkNumber(Part asmPart)
        {
            List<int> number = new List<int>();

            List<Part> works = ElectrodeAssembleCollection.GetWorkCollection(asmPart);
            foreach (Part part in works)
            {
                WorkAssembleModel mold = new WorkAssembleModel();
                mold.GetPart(part);
                number.Add(mold.WorkNumber);
            }
            return number;
        }
        /// <summary>
        /// 添加work
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="number"></param>
        /// <param name="asmPart"></param>
        public static void CreateWork(Matrix4 matr, int number, Part asmPart)
        {
            AsmAssembleModel asmMold = new AsmAssembleModel();
            EDMAssembleModel edmMold = new EDMAssembleModel();
            asmMold.GetPart(asmPart);
            edmMold.GetPart(ElectrodeAssembleCollection.GetEDMCollection(asmPart));
            WorkAssembleModel work = new WorkAssembleModel(asmMold.MoldInfo, number, matr);
            work.CreatePart(asmMold.WorkpieceDirectoryPath);
            PartUtils.SetPartDisplay(asmPart);
            NXOpen.Assemblies.Component comp = work.Load(asmPart);
            edmMold.Load(work.PartTag);
            PartUtils.SetPartWork(comp);
        }
        /// <summary>
        /// 修改矩阵
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="number"></param>
        /// <param name="asmPart"></param>
        public static void AlterMatr(Matrix4 matr, int number, Part asmPart)
        {
            Tag csysTag = Tag.Null;
            UFSession theUFSession = UFSession.GetUFSession();
            AsmAssembleModel asmMold = new AsmAssembleModel();
            WorkAssembleModel work = new WorkAssembleModel();
            asmMold.GetPart(asmPart);
            List<Part> works = ElectrodeAssembleCollection.GetWorkCollection(asmPart);
            string workName = asmMold.MoldInfo.MoldNumber + "-" + asmMold.MoldInfo.WorkpieceNumber + "-WORK" + number.ToString();
            foreach (Part part in works)
            {
                if (part.Name.Equals(workName))
                {
                    work.GetPart(part);
                    break;
                }
            }
            work.AlterMatr(matr);
            PartUtils.SetPartDisplay(work.PartTag);
            theUFSession.Obj.CycleByName("WORK" + number.ToString(), ref csysTag);
            theUFSession.Obj.DeleteObject(csysTag);
        }

    }
}
