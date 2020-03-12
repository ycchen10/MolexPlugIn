using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using Basic;


namespace MolexPlugin.Model
{
    public abstract class AbstractModel : IEquatable<AbstractModel>
    {
        /// <summary>
        /// 模具信息
        /// </summary>
        public MoldInfoModel MoldInfo { get; set; } = new MoldInfoModel();
        /// <summary>
        /// part档类型
        /// </summary>
        public string PartType { get; protected set; }
        /// <summary>
        /// 工件
        /// </summary>
        public Part PartTag { get; protected set; }
        /// <summary>
        /// 工件名
        /// </summary>
        public string AssembleName { get; protected set; }
        /// <summary>
        /// 工件地址
        /// </summary>
        public string WorkpiecePath { get; protected set; }
        /// <summary>
        /// 文件夹位置
        /// </summary>
        public string WorkpieceDirectoryPath { get; protected set; }

        public bool Equals(AbstractModel other)
        {
            return this.AssembleName.Equals(other.AssembleName);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        protected virtual void SetAttribute()
        {
            MoldInfo.SetAttribute(this.PartTag);
            AttributeUtils.AttributeOperation("PartType", this.PartType, this.PartTag);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="part"></param>
        protected virtual void GetAttribute(Part part)
        {
            this.PartTag = part;
            MoldInfo.GetAttribute(part);
            this.PartType = AttributeUtils.GetAttrForString(part, "PartType");
        }

        /// <summary>
        /// 获取名字
        /// </summary>
        public abstract void GetAssembleName();
        /// <summary>
        /// 创建part档
        /// </summary>
        /// <returns></returns>
        public abstract void CreatePart();
        /// <summary>
        /// 通过part获得Model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public abstract void GetModelForPart(Part part);
        /// <summary>
        /// 装配
        /// </summary>
        /// <param name="parentPart"></param>
        /// <returns></returns>
        public abstract NXOpen.Assemblies.Component Load(Part parentPart);

    }
}
