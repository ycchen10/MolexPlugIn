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
        public AssembleModel Model { get; private set; }
        private Part asmPart;
        private AssembleSingleton singleton;
        public AddWorkBuilder(Part asmPart)
        {
            this.asmPart = asmPart;
            MoldInfoModel mold = new MoldInfoModel(asmPart);
            string name = mold.MoldNumber + "-" + mold.WorkpieceNumber;
            singleton = AssembleSingleton.Instance();
            this.Model = singleton.GetAssemble(name);
        }
        /// <summary>
        /// 创建特征
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="workNumber"></param>
        public bool CreateBuilder(Matrix4 mat, int workNumber)
        {

            CreateWorkPart work = new CreateWorkPart(this.Model.Asm.WorkpieceDirectoryPath, this.Model.Asm.MoldInfo, workNumber, mat);
            if (!work.CreatePart())
                return false;
            NXOpen.Assemblies.Component workComp = work.Load(asmPart);
            PartUtils.SetPartDisplay(asmPart);
            PartUtils.SetPartWork(workComp);

            Model.Edm.Load(work.Model.PartTag);
            CartesianCoordinateSystem csys = asmPart.WCS.Save();
            csys.Name = "WORK" + workNumber.ToString();
            csys.Layer = 200;
            csys.Color = 186;
            PartUtils.SetPartDisplay(asmPart);
            singleton.AddWork(work.Model);
            return true;
        }

        /// <summary>
        /// 修改矩阵
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="workNumber"></param>
        /// <returns></returns>
        public bool AlterMatr(Matrix4 mat, int workNumber)
        {
            WorkModel work = this.Model.Works.Find(x => x.WorkNumber == workNumber);
            if (work != null)
            {
                NXOpen.Assemblies.Component workComp = work.PartTag.OwningComponent;
                PartUtils.SetPartWork(workComp);
                DeleCsys(workNumber);
                work.AlterMatr(mat);
                CartesianCoordinateSystem csys = work.PartTag.WCS.Save();
                csys.Name = "WORK" + workNumber.ToString();
                csys.Color = 186;
                csys.Layer = 200;
                return true;
            }
            return false;

        }
        /// <summary>
        /// 获取Work号
        /// </summary>
        /// <returns></returns>
        public List<int> GetWorkNumber()
        {
            List<int> temp = new List<int>();
            this.Model.Works.Sort();
            foreach (WorkModel wm in this.Model.Works)
            {
                temp.Add(wm.WorkNumber);
            }
            return temp;
        }


        private void DeleCsys(int workNumber)
        {
            string csysName = "WORK" + workNumber.ToString();
            UFSession theUFSessino = UFSession.GetUFSession();
            Tag csys = Tag.Null;
            theUFSessino.Obj.CycleByName(csysName, ref csys);
            if (csys != Tag.Null)
            {
                theUFSessino.Obj.DeleteObject(csys);

            }
        }
    }
}
