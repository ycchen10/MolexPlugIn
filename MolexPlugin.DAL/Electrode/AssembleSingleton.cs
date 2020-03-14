using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 装配单列
    /// </summary>
    public class AssembleSingleton
    {
        public static Dictionary<string, AssembleModel> Assembles { get; private set; } = new Dictionary<string, AssembleModel>();

        private static AssembleSingleton instance = null;

        private static object singletonLock=new object();
        private AssembleSingleton()
        {

        }
        public static AssembleSingleton Instance()
        {
            if (instance == null)
            {
                lock (singletonLock)
                {
                    if (instance == null)
                    {
                        instance = new AssembleSingleton();
                    }
                }
            }
            return instance;
        }
        /// <summary>
        /// 获取装配
        /// </summary>
        /// <returns></returns>
        public AssembleModel GetAssemble(string moldAndWorkpieceNum)
        {
            string asm = moldAndWorkpieceNum+"-ASM";
            if (AssembleSingleton.Assembles.ContainsKey(asm))
                return AssembleSingleton.Assembles[asm];
            else
            {
               
                AssembleModel model = new AssembleModel(moldAndWorkpieceNum);
                Assembles.Add(asm, model);
                return model;

            }
        }
        /// <summary>
        /// 添加Work
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        public bool AddWork(WorkModel work)
        {

            string asm = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber + "-ASM";
            if (AssembleSingleton.Assembles.ContainsKey(asm))
            {
                AssembleSingleton.Assembles[asm].AddWork(work);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加电极
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public bool AddElectrode(ElectrodeModel ele)
        {
            string asm = ele.MoldInfo.MoldNumber + "-" + ele.MoldInfo.WorkpieceNumber + "-ASM";
            if (AssembleSingleton.Assembles.ContainsKey(asm))
            {
                AssembleSingleton.Assembles[asm].AddElectrode(ele);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断装配完整性
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public bool AskAssembleFull(string asm)
        {
            if (AssembleSingleton.Assembles.ContainsKey(asm))
            {
                if (AssembleSingleton.Assembles[asm].Edm != null && (AssembleSingleton.Assembles[asm].Works.Count != 0)
                     && AssembleSingleton.Assembles[asm].Asm != null)
                    return true;
            }
            return false;
        }
     
    }
}
