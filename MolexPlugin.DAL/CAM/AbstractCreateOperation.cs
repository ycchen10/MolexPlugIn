using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using NXOpen.BlockStyler;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public abstract class AbstractCreateOperation : IComparable<AbstractCreateOperation>
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int Site { get; protected set; }
        /// <summary>
        /// 刀具名
        /// </summary>
        public string ToolName { get; protected set; }
        /// <summary>
        /// 刀路名字
        /// </summary>
        public OperationNameModel NameModel { get; protected set; } = null;

        public AbstractOperationModel Oper { get; protected set; } = null;

        public Node Node { get; set; }
        public AbstractCreateOperation(int site, string tool)
        {
            this.Site = site;
            this.ToolName = tool;
        }
        /// <summary>
        /// 创建刀路名
        /// </summary>
        /// <param name="tool"></param>
        public abstract void CreateOperationName();
        /// <summary>
        /// 创建刀路
        /// </summary>
        /// <param name="eleCam"></param>
        /// <param name="inter">电极间隙</param>
        public abstract void CreateOperation(ElectrodeCAM eleCam, double inter);
        /// <summary>
        /// 拷贝刀路
        /// </summary>
        /// <returns></returns>
        public abstract AbstractCreateOperation CopyOperation();
        /// <summary>
        /// 设置刀具名
        /// </summary>
        /// <param name="tool"></param>
        public void SetToolName(string tool)
        {
            this.NameModel.ToolName = tool;
        }
        /// <summary>
        /// 设置程序名
        /// </summary>
        /// <param name="program"></param>
        public void SetProgramName(int site)
        {
            this.Site = site;
            this.NameModel.ProgramName = "O000" + site.ToString();
        }

        public int CompareTo(AbstractCreateOperation other)
        {
            return this.Site.CompareTo(other.Site);

        }

    }
}
