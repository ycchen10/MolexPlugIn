using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极刀具路径抽象类
    /// </summary>
    public abstract class AbstractElectrodeOperation
    {

        public List<AbstractCreateOperation> Oper { get; protected set; } = new List<AbstractCreateOperation>();
        public ElectrodeCAM Cam { get; private set; }

        public ElectrodeCAMInfo CamInfo { get; set; }

        public ElectrodeModel EleModel { get; set; }

        protected ComputeTool tool;
        public AbstractElectrodeOperation(ElectrodeModel ele, ElectrodeCAMInfo info)
        {
            this.EleModel = ele;
            Part workPart = Session.GetSession().Parts.Work;
            CamInfo = info;
            tool = new ComputeTool(CamInfo.MinDim, CamInfo.MinDia);
            Cam = new ElectrodeCAM();
        }
        /// <summary>
        /// 添加刀路名
        /// </summary>
        /// <param name="model"></param>
        public void AddOperationNameModel(AbstractCreateOperation model, int count)
        {
            Oper.IndexOf(model, count);
        }
        /// <summary>
        /// 删除刀路
        /// </summary>
        /// <param name="model"></param>
        public void DeleteOperationNameModel(AbstractCreateOperation model)
        {
            Oper.Remove(model);
        }

        public void DeleteOperationNameModel(int count)
        {
            Oper.RemoveAt(count);
        }

        public void UpdateOperationNameModel()
        {
            for (int i = 0; i < Oper.Count; i++)
            {
                Oper[i].SetProgramName(i + 1);
            }
        }
        /// <summary>
        /// 设置加工体
        /// </summary>
        protected void SetWorkpiece()
        {
            CAMUtils.SetFeatureGeometry("WORKPIECE", this.EleModel.PartTag.Bodies.ToArray());
        }
        /// <summary>
        /// 设置加工环境
        /// </summary>
        protected void CreateCamSetup()
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;

            theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");

            bool result1;
            result1 = theSession.IsCamSessionInitialized();

            theSession.CreateCamSession();

            NXOpen.CAM.CAMSetup cAMSetup1;
            cAMSetup1 = workPart.CreateCamSetup("electrode");
           
        }
        /// <summary>
        /// 创建加工操作
        /// </summary>
        public void CreateOperation(bool isInter)
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (workPart.Tag != this.EleModel.PartTag.Tag)
                PartUtils.SetPartDisplay(this.EleModel.PartTag);
            this.CreateCamSetup();
            this.SetWorkpiece();
            DeleteObject.UpdateObject();
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.CreateOperation(Cam, GetInter(isInter));
            }
        }
        /// <summary>
        ///  创建加工名字
        /// </summary>
        public void CreateOperationNameModel()
        {
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.CreateOperationName();
            }

        }     
        protected double GetInter(bool isInter)
        {
            if (isInter)
                return 0;
            else
            {
                if (this.EleModel.EleInfo.FineInter != 0)
                    return this.EleModel.EleInfo.FineInter;
                if (this.EleModel.EleInfo.DuringInter != 0)
                    return this.EleModel.EleInfo.DuringInter;
                if (this.EleModel.EleInfo.CrudeInter != 0)
                    return this.EleModel.EleInfo.CrudeInter;
            }
            return 0;
        }

    }
}
