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
    /// 后处理电极信息
    /// </summary>
    public class PostElectrodenfo : PostPartInfo
    {


        public PostElectrodenfo(Part part) : base(part)
        {
        }  
      
        /// <summary>
        /// 获取电极信息
        /// </summary>
        /// <returns></returns>
        public ElectrodeModel GetElectrodeModel()
        {
            ElectrodeModel model = new ElectrodeModel();
            model.GetModelForPart(part);
            return model;
        }
        /// <summary>
        /// 获取是否扣间隙
        /// </summary>
        /// <returns></returns>
        public bool GetIsInter()
        {
            return AttributeUtils.GetAttrForBool(part, "Inter");
        }

    }
}
