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

        public List<AbstractCreateOperation> Oper { get; protected set; }
        public ElectrodeCAM Cam { get; private set; }

        protected ElectrodeCAMInfo camInfo;
        private ElectrodeModel ele;

        protected ComputeTool tool;
        public AbstractElectrodeOperation(ElectrodeModel ele, bool isInter)
        {
            this.ele = ele;
            Part workPart = Session.GetSession().Parts.Work;
            camInfo = new ElectrodeCAMInfo(ele, isInter);
            tool = new ComputeTool(camInfo.MinDim, camInfo.MinDia);
            if (workPart.Tag != ele.PartTag.Tag)
                PartUtils.SetPartDisplay(ele.PartTag);
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
        /// 设置加工体
        /// </summary>
        protected void SetWorkpiece()
        {
            CAMUtils.SetFeatureGeometry("WORKPIECE", ele.PartTag.Bodies.ToArray());
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
            NXOpen.CAM.CAMSetup cAMSetup1;
            cAMSetup1 = workPart.CreateCamSetup("electrode");
        }
        /// <summary>
        /// 创建加工名字
        /// </summary>
        public abstract void CreateOperationNameModel();
        /// <summary>
        /// 创建加工操作
        /// </summary>
        public abstract void CreateOperation();

    }
}
