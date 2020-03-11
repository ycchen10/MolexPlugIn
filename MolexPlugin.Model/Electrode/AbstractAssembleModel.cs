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
    public abstract class AbstractAssembleModel : IEquatable<AbstractAssembleModel>
    {
        public MoldInfoModel MoldInfo { get; set; } = new MoldInfoModel();

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

        public AbstractAssembleModel()
        {

        }
        /// <summary>
        /// 写入属性
        /// </summary>
        protected virtual void SetAttribute()
        {
            MoldInfo.SetAttribute(this.PartTag);
            AttributeUtils.AttributeOperation("PartType", this.PartType, this.PartTag);
        }
        /// <summary>
        /// 读取属性
        /// </summary>
        protected virtual void GetAttribute(Part part)
        {
            this.PartTag = part;
            MoldInfo.GetAttribute(this.PartTag);
            this.PartType = AttributeUtils.GetAttrForString(part, "PartType");
        }
        public virtual void GetAssembleName()
        {
            this.AssembleName = this.MoldInfo.MoldNumber + "-" + this.MoldInfo.WorkpieceNumber;
        }
        /// <summary>
        /// 创建工件
        /// </summary>
        public virtual bool CreatePart(string filePath)
        {
            GetAssembleName();
            this.WorkpieceDirectoryPath = filePath;
            this.WorkpiecePath = filePath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                File.Delete(this.WorkpiecePath);
            }
            Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
            this.PartTag = part;
            SetAttribute();
            return true;
        }
        /// <summary>
        /// 获取Part属性
        /// </summary>
        /// <param name="part"></param>
        public virtual void GetPart(Part part)
        {
            this.GetAttribute(part);
            this.AssembleName = part.Name;
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
            this.AssembleName = Path.GetFileNameWithoutExtension(this.WorkpiecePath);

        }
        /// <summary>
        /// 加载组立
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual NXOpen.Assemblies.Component Load(Part part)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            return Basic.AssmbliesUtils.PartLoad(part, this.WorkpiecePath, this.AssembleName, matr, new Point3d(0, 0, 0));
        }

        public bool Equals(AbstractAssembleModel other)
        {
            return this.AssembleName.Equals(other.AssembleName);
        }
    }
}
