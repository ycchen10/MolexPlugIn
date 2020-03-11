using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodeFactory
    {
        private ElectrodeHeadModel head;
        private ElectrodeInfo info;
        private AssembleCollection coll;
        private ElectrodePart elePart;
        public ElectrodeFactory(ElectrodeHeadModel head, ElectrodeInfo info)
        {
            this.head = head;
            this.info = info;
            AssembleInstance inst = AssembleInstance.GetInstance();
            coll = inst.GetAssembleModle();

        }

        public void CreateEle()
        {
            Session theSession = Session.GetSession();

            Part work = theSession.Parts.Work;
            MoldInfoModel mold = new MoldInfoModel(work);
            MoveObjectLayer(info.EleNumber + 100, head.model.Bodys.ToArray());
            elePart = new ElectrodePart(head, info, mold, work);
            elePart.CreateEle();
            if (elePart.IsOk)
            {
                PartUtils.SetPartWork(null); //设置WORK是显示部件
                MoveEleComp(elePart.EleComp);
            }
            CsysUtils.SetWcsToAbs();
            DeleteObject.UpdateObject();

        }

        /// <summary>
        /// 移动装配档电极
        /// </summary>
        /// <param name="eleComp"></param>
        public void MoveEleComp(NXOpen.Assemblies.Component eleComp)
        {

            AssmbliesUtils.MoveCompPart(eleComp, elePart.GetMovePoint(), head.model.Work.Matr);
        }
        /// <summary>
        /// 移动电极头
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="objs"></param>
        private void MoveObjectLayer(int layer, params NXObject[] objs)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (NXObject obj in objs)
            {
                theUFSession.Obj.SetLayer(obj.Tag, layer);
            }

        }



    }
}
