using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建刀路逻辑
    /// </summary>
    public class CreateElectrodeCAM
    {
        public ElectrodeModel EleModel { get; private set; }

        public AbstractElectrodeOperation EleOper { get; private set; }

        public ElectrodeCAMInfo CamInfo { get; set; }

        private string template;
        public CreateElectrodeCAM(ElectrodeModel model, string template)
        {
            this.EleModel = model;
            this.CamInfo = new ElectrodeCAMInfo(model);
            this.template = template;
            EleOper = ElectrodeOperationFactory.CreateOperation(template, model, CamInfo);
        }
        /// <summary>
        /// 创建电极文件夹
        /// </summary>
        /// <param name="filePath"></param>
        private string CreateEleFile(string filePath)
        {
            string newFilePath = filePath + "\\" + EleModel.EleInfo.EleName + "\\";
            if (Directory.Exists(newFilePath))
            {
                Directory.Delete(newFilePath, true);
            }
            Directory.CreateDirectory(newFilePath); //创建电极文件夹
            return newFilePath;
        }
        /// <summary>
        /// 偏置电极间隙
        /// </summary>
        /// <param name="part"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private bool SetOffsetInter(Part part, double side)
        {
            bool isok = false;
            UFSession theUFSession = UFSession.GetUFSession();
            List<Tag> featureTags = new List<Tag>();
            Tag groupTag;
            foreach (NXOpen.Features.Feature fe in part.Features)
            {
                featureTags.Add(fe.Tag);
            }
            theUFSession.Modl.CreateSetOfFeature("电极特征", featureTags.ToArray(), featureTags.Count, 1, out groupTag);

            NXObject obj = OffsetRegion.Offset(side, out isok, part.Bodies.ToArray()[0].GetFaces());
            if (isok)
            {
                obj.SetName(side.ToString());
                AttributeUtils.AttributeOperation("Inter", isok, part);

            }
            return isok;
        }
        /// <summary>
        /// 获取电极信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, double> GetEleInter()
        {
            Dictionary<string, double> inters = new Dictionary<string, double>();
            if (EleModel.EleInfo.FineInter != 0)
                inters.Add("F", this.EleModel.EleInfo.FineInter);
            if (EleModel.EleInfo.DuringInter != 0)
                inters.Add("D", this.EleModel.EleInfo.DuringInter);
            if (EleModel.EleInfo.CrudeInter != 0)
                inters.Add("R", this.EleModel.EleInfo.CrudeInter);
            return inters;
        }
        private string CoypeEle(string filePath, Dictionary<string, double> inters)
        {
            Session theSession = Session.GetSession();
            PartLoadStatus status;
            string newElePath = "";
            if (inters.Count == 1)
            {
                newElePath = filePath + EleModel.EleInfo.EleName + ".prt";
            }
            else
            {
                string key = inters.Keys.ToArray()[0];
                string newPath = filePath + key + "\\";
                newElePath = newPath + EleModel.EleInfo.EleName + "-" + key + ".prt";

            }
            File.Move(EleModel.WorkpiecePath, newElePath);
            Part dis = theSession.Parts.OpenDisplay(newElePath, out status);
            this.EleModel.GetModelForPart(dis);
            return newElePath;
        }
        private bool OffsetInter(Dictionary<string, double> inters)
        {
            double inter = 0;

            if (inters.Count == 1)
            {
                string key = inters.Keys.ToArray()[0];

                inter = inters[key];
            }
            else
            {
                string key = inters.Keys.ToArray()[0];
                inter = inters[key];
            }
            return SetOffsetInter(this.EleModel.PartTag, -inter);
        }
        /// <summary>
        /// 创建名字
        /// </summary>
        public void CreateOperName()
        {        
            this.EleOper.CreateOperationNameModel();
        }
        /// <summary>
        /// 创建操作
        /// </summary>
        public void CreateOper()
        {
            PartUtils.SetPartDisplay(this.EleModel.PartTag);
            Session theSession = Session.GetSession();
            try
            {
                if (this.EleModel.PartTag.CAMSetup != null)
                    return;
            }
            catch
            {
                Dictionary<string, double> inters = GetEleInter();
                bool isok = OffsetInter(inters);
                this.EleOper.CreateOperation(isok);
                theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
                EleModel.PartTag.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
            }
        }
        /// <summary>
        /// 计算刀路
        /// </summary>
        public void SetGenerateToolPath()
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
            NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
            workPart.CAMSetup.GenerateToolPath(new CAMObject[1] { nCGroup1 });
            EleModel.PartTag.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
        }

        public void CopyEle(string filePath)
        {
            Dictionary<string, double> inters = GetEleInter();
            string file = CreateEleFile(filePath);
            CoypeEle(file, inters);
        }
    }
}
