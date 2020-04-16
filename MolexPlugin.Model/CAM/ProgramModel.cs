using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
namespace MolexPlugin.Model
{
    public class ProgramModel : IComparable<ProgramModel>
    {
        /// <summary>
        /// 程序组
        /// </summary>
        public NCGroup ProgramGroup { get; private set; }
        /// <summary>
        /// 程序组是否正确
        /// </summary>
        public bool Estimate { get; private set; } = true;
        /// <summary>
        /// 刀具路径数据
        /// </summary>
        public List<OperationData> OperData { get; private set; }
        public ProgramModel(NCGroup program)
        {
            this.ProgramGroup = program;
            GetOper();
        }
        /// <summary>
        /// 获取刀具路径数据
        /// </summary>
        private void GetOper()
        {
            List<OperationData> data = new List<OperationData>();
            string toolName = "";
            foreach (CAMObject np in this.ProgramGroup.GetMembers())
            {
                if (np is NXOpen.CAM.Operation)
                {
                    AbstractOperationModel model = CreateOperationFactory.GetOperation(np as NXOpen.CAM.Operation);
                    OperationData od = model.GetOperationData();
                    data.Add(od);
                    if (toolName == "")
                        toolName = od.Tool.ToolName;
                    else if (!toolName.ToUpper().Equals(od.Tool.ToolName.ToUpper()))
                    {
                        Estimate = false;
                        break;
                    }
                }

            }
            this.OperData = data;
        }
        public int CompareTo(ProgramModel other)
        {
            return this.ProgramGroup.Name.CompareTo(other.ProgramGroup.Name);
        }
    }
}
