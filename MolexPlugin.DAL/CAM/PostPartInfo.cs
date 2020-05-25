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
    /// 后处理part信息
    /// </summary>
    public class PostPartInfo
    {
        protected Part part;

        public PostPartInfo(Part part)
        {
            this.part = part;
        }
        /// <summary>
        /// 判断Part是否有信息
        /// </summary>
        /// <returns></returns>
        public static bool IsPartInfo(Part part)
        {
            string type = AttributeUtils.GetAttrForString(part, "PartType");
            if (type.Equals(""))
                return false;
            else
                return true;
        }
        /// <summary>
        /// 判断Part是否是电极
        /// </summary>
        /// <returns></returns>
        public static bool IsPartElectrode(Part part)
        {
            string type = AttributeUtils.GetAttrForString(part, "PartType");
            if (type.Equals("Electrode", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取模具信息
        /// </summary>
        /// <returns></returns>
        public MoldInfoModel GetPartMoldInfo()
        {
            return new MoldInfoModel(part);
        }
        /// <summary>
        /// 获取CAM用户
        /// </summary>
        /// <returns></returns>
        public string GetCamUser()
        {
            return AttributeUtils.GetAttrForString(part, "CAMUser");
        }
    }
}
