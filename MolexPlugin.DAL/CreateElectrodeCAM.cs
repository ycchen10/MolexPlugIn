using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
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

        private ElectrodeCAMInfo camInfo;

        public CreateElectrodeCAM(ElectrodeModel model)
        {
            this.EleModel = model;
            this.camInfo = new ElectrodeCAMInfo(model);
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

            OffsetRegion.Offset(side, out isok, part.Bodies.ToArray()[0].GetFaces());
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

        private bool CoypeEle(Dictionary<string, double> inters)
        {
            Session theSession = Session.GetSession();
            //  PartLoadStatus status;
            double inter = 0;
            // string newElePath = "";
            if (inters.Count == 1)
            {
                string key = inters.Keys.ToArray()[0];
                //  newElePath = filePath + eleModel.EleInfo.EleName + ".prt";
                inter = inters[key];
            }
            else
            {
                string key = inters.Keys.ToArray()[0];
                //  string newPath = filePath + key + "\\";
                //  newElePath = newPath + eleModel.EleInfo.EleName + "-" + key + ".prt";
                inter = inters[key];
            }
            //  File.Copy(eleModel.WorkpiecePath, newElePath);
            //  Part dis = theSession.Parts.OpenDisplay(newElePath, out status);
            //   this.eleModel.GetModelForPart(dis);
            //   this.EleOper.EleModel = this.eleModel;
            //   this.EleOper.CamInfo = new ElectrodeCAMInfo(this.eleModel);
            return SetOffsetInter(this.EleModel.PartTag, -inter);
        }
        /// <summary>
        /// 创建名字
        /// </summary>
        public void CreateOperName()
        {
            if (camInfo.GetFlatFaces().Count == 0 && camInfo.GetSteepFaces().Count == 0)
            {
                this.EleOper = new SimplenessVerticalEleOperation(this.EleModel, this.camInfo);
                this.EleOper.CreateOperationNameModel();
            }
            this.EleOper = new SimplenessVerticalEleOperation(this.EleModel, this.camInfo);
            this.EleOper.CreateOperationNameModel();
        }
        /// <summary>
        /// 创建操作
        /// </summary>
        public void CreateOper()
        {
            // string newFilePath = CreateEleFile(filePath);
            PartUtils.SetPartDisplay(this.EleModel.PartTag);
            Dictionary<string, double> inters = GetEleInter();
            bool isok = CoypeEle(inters);

            this.EleOper.CreateOperation(isok);
        }
    }
}
