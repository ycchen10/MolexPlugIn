using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 模具信息
    /// </summary>
    public class MoldInfoModel
    {
        /// <summary>
        /// 模号
        /// </summary>
        public string MoldNumber { get; set; }
        /// <summary>
        /// 件号
        /// </summary>
        public string WorkpieceNumber { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string EditionNumber { get; set; }
        /// <summary>
        /// 模具类型
        /// </summary>
        public string MoldType { get; set; }
        /// <summary>
        /// 客户名
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 创建者名
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string CreatedDate { get; set; }

        public MoldInfoModel()
        {

        }

        public MoldInfoModel(string mold, string piece, string edition, string type, string client, string name, string data)
        {
            this.EditionNumber = edition;
            this.MoldNumber = mold;
            this.WorkpieceNumber = piece;
            this.MoldType = type;
            this.ClientName = client;
            this.CreatorName = name;
            this.CreatedDate = data;
        }
        public MoldInfoModel(Part part)
        {
            this.GetAttribute(part);
        }

        public virtual void SetAttribute(Part part)
        {
            AttributeUtils.AttributeOperation("MoldNumber", this.MoldNumber, part);
            AttributeUtils.AttributeOperation("PieceNumber", this.WorkpieceNumber, part);
            AttributeUtils.AttributeOperation("EditionNumber", this.EditionNumber, part);
            AttributeUtils.AttributeOperation("MoldType", this.MoldType, part);
            AttributeUtils.AttributeOperation("ClientName", this.ClientName, part);
            AttributeUtils.AttributeOperation("CreatorName", this.CreatorName, part);
            AttributeUtils.AttributeOperation("CreatedDate", this.CreatedDate, part);
        }
        public void GetAttribute(Part part)
        {
            this.MoldNumber = AttributeUtils.GetAttrForString(part, "MoldNumber");
            this.WorkpieceNumber = AttributeUtils.GetAttrForString(part, "PieceNumber");
            this.EditionNumber = AttributeUtils.GetAttrForString(part, "EditionNumber");
            this.MoldType = AttributeUtils.GetAttrForString(part, "MoldType");
            this.ClientName = AttributeUtils.GetAttrForString(part, "ClientName");
            this.CreatorName = AttributeUtils.GetAttrForString(part, "CreatorName");
            this.CreatedDate = AttributeUtils.GetAttrForString(part, "CreatedDate");
        }
    }
}
