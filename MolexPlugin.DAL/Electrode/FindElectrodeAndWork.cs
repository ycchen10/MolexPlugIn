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
    public class FindElectrodeAndWork
    {
        /// <summary>
        /// 通过WORK找电极
        /// </summary>
        /// <param name="workNum"></param>
        /// <returns></returns>
        public static List<ElectrodeModel> FindElectrodeForWork(WorkModel work)
        {
            List<ElectrodeModel> models = new List<ElectrodeModel>();
            string name = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber;
            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.Length > name.Length)
                {
                    int number = AttributeUtils.GetAttrForInt(part, "WorkNumber");
                    string type = AttributeUtils.GetAttrForString(part, "PartType");
                    if (work.WorkNumber == number && type == "Electrode" && part.Name.Substring(0, name.Length).Equals(name))
                    {
                        ElectrodeModel model = new ElectrodeModel();
                        model.GetModelForPart(part);
                        models.Add(model);
                    }
                }
            }
            return models;
        }
        /// <summary>
        /// 通过电极找Work
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static WorkModel FindWorkForElectrode(ElectrodeModel ele)
        {
            string name = ele.MoldInfo.MoldNumber + "-" + ele.MoldInfo.WorkpieceNumber;
            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.Length > name.Length)
                {
                    int number = AttributeUtils.GetAttrForInt(part, "WorkNumber");
                    string type = AttributeUtils.GetAttrForString(part, "PartType");
                    if (ele.WorkNumber == number && type == "Work" && part.Name.Substring(0, name.Length).Equals(name))
                    {
                        WorkModel model = new WorkModel();
                        model.GetModelForPart(part);
                        return model;
                    }
                }
            }
            return null;
        }
    }
}
