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
    /// <summary>
    /// 检查装配环境。
    /// </summary>
    public class AssembleInstance
    {
        private static Dictionary<string, AssembleCollection> modle = new Dictionary<string, AssembleCollection>();

        private static AssembleInstance instance = null;

        private static object syncLocker = new object();
        private AssembleInstance()
        {

        }
        public static AssembleInstance GetInstance()
        {
            if (instance == null)
            { 
                lock (syncLocker)
                {
                    if (instance == null)
                        instance = new AssembleInstance();
                }
            }
            return instance;
        }
        /// <summary>
        /// 获取装配
        /// </summary>
        /// <returns></returns>
        public AssembleCollection GetAssembleModle()
        {

            string asm = GetAsmName();
            if (AssembleInstance.modle.ContainsKey(asm))
                return AssembleInstance.modle[asm];
            else
            {
                return new AssembleCollection();

            }
        }
        /// <summary>
        /// 添加WORK
        /// </summary>
        /// <param name="work"></param>
        public void AddWork(WorkAssembleModel work)
        {
            AssembleCollection modle = GetAssembleModle();
            modle.AddWork(work);
            
        }
        /// <summary>
        /// 添加电极
        /// </summary>
        /// <param name="ele"></param>
        public void AddEle(ElectrodeAssembleModel ele)
        {
            AssembleCollection modle = GetAssembleModle();
            modle.AddEle(ele);

        }
        /// <summary>
        /// 获取ASM
        /// </summary>
        /// <returns></returns>
        private string GetAsmName()
        {
            Part workPart = Session.GetSession().Parts.Work;
            MoldInfoModel info = new MoldInfoModel(workPart);
            return info.MoldNumber + "-" + info.WorkpieceNumber + "-ASM";
        }

    }
}
