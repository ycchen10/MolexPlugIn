using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ExportFile
    {
        private ElectrodeModel model;
        public ExportFile(ElectrodeModel model)
        {
            this.model = model;
        }

        private string CreateEleFile(string filePath)
        {
            string newFilePath = filePath + "\\" + model.EleInfo.EleName + "\\";
            if (Directory.Exists(newFilePath))
            {
                Directory.Delete(newFilePath, true);
            }
            Directory.CreateDirectory(newFilePath); //创建电极文件夹
            return newFilePath;
        }
        /// <summary>
        /// 获取电极信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, double> GetEleInter()
        {
            Dictionary<string, double> inters = new Dictionary<string, double>();
            if (model.EleInfo.FineInter != 0)
                inters.Add("F", this.model.EleInfo.FineInter);
            if (model.EleInfo.DuringInter != 0)
                inters.Add("D", this.model.EleInfo.DuringInter);
            if (model.EleInfo.CrudeInter != 0)
                inters.Add("R", this.model.EleInfo.CrudeInter);
            return inters;
        }
        private string CoypeEle(string filePath, Dictionary<string, double> inters)
        {
            Session theSession = Session.GetSession();
            string newElePath = "";
            if (inters.Count == 1)
            {
                newElePath = filePath + model.EleInfo.EleName + ".prt";
            }
            else
            {
                string key = inters.Keys.ToArray()[0];
                string newPath = filePath + key + "\\";
                if (Directory.Exists(newPath))
                {
                    Directory.Delete(newPath, true);
                }
                Directory.CreateDirectory(newPath); //F
                newElePath = newPath + model.EleInfo.EleName + "-" + key + ".prt";

            }
            File.Copy(model.WorkpiecePath, newElePath);
            return newElePath;
        }

        public string NewFile(string file)
        {
            string newFile = CreateEleFile(file);
            return CoypeEle(newFile, GetEleInter());
        }

    }
}
