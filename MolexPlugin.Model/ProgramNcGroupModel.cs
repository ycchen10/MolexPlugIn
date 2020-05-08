using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 程序组
    /// </summary>
    public class ProgramNcGroupModel
    {
        private NCGroup program;
        public ProgramNcGroupModel(NCGroup np)
        {
            this.program = np;
        }
        /// <summary>
        /// 判断全否程序组
        /// </summary>
        /// <returns></returns>
        public bool IsProgram()
        {
            bool isok = true;
            foreach (CAMObject ct in this.program.GetMembers())
            {
                if (!(ct is NCGroup))
                {
                    isok = false;
                    break;
                }
            }
            return isok;
        }
        /// <summary>
        ///获取程序组
        /// </summary>
        /// <returns></returns>
        public List<NCGroup> GetProgram()
        {
            List<NCGroup> programGroup = new List<NCGroup>();
            foreach (CAMObject ct in this.program.GetMembers())
            {
                if (ct is NCGroup)
                {
                    programGroup.Add(ct as NCGroup);
                }
            }
            return programGroup;
        }
        /// <summary>
        /// 判断是否全是操作
        /// </summary>
        /// <returns></returns>
        public bool IsOperation()
        {
            bool isok = true;
            foreach (CAMObject ct in this.program.GetMembers())
            {
                if (!(ct is NXOpen.CAM.Operation))
                {
                    isok = false;
                    break;
                }
            }
            return isok;
        }
        /// <summary>
        /// 获取操作
        /// </summary>
        /// <returns></returns>
        public List<NXOpen.CAM.Operation> GetOperation()
        {
            List<NXOpen.CAM.Operation> operGroup = new List<NXOpen.CAM.Operation>();
            foreach (CAMObject ct in this.program.GetMembers())
            {
                if (ct is NXOpen.CAM.Operation)
                {

                    operGroup.Add(ct as NXOpen.CAM.Operation);

                }
            }
            return operGroup;
        }
        /// <summary>
        /// 判断操作是否是同一把刀
        /// </summary>
        /// <returns></returns>
        public bool IsOperationEqualTool()
        {
            string toolName = "";
            bool isok = true;
            foreach (NXOpen.CAM.Operation op in GetOperation())
            {
                NCGroup tool = op.GetParent(CAMSetup.View.MachineTool);
                if (toolName.Equals(""))
                    toolName = tool.Name;
                else if (!tool.Name.Equals(toolName, StringComparison.CurrentCultureIgnoreCase))
                {
                    isok = false;
                    break;
                }

            }
            return isok;
        }

        public List<OperationData> GetOperationData()
        {
            List<OperationData> data = new List<OperationData>();
            foreach (NXOpen.CAM.Operation op in GetOperation())
            {
                AbstractOperationModel model = CreateOperationFactory.GetOperation(op);
                data.Add(model.GetOperationData());
            }
            return data;
        }
        /// <summary>
        /// 获取最大最小Z向深度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="zMin"></param>
        /// <param name="zMax"></param>
        public void GetOperationZMinAndZMax(List<OperationData> data, out double zMin, out double zMax)
        {
            zMin = 0;
            zMax = -9999;
            foreach (OperationData da in data)
            {
                if (da.Zmax > zMax)
                    zMax = da.Zmax;
                if (da.Zmin < zMin)
                    zMin = da.Zmin;
            }
        }
        /// <summary>
        /// 获取程序组时间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetProgramTime(List<OperationData> data)
        {
            double time = 0;
            foreach (OperationData od in data)
            {
                time += od.OperTime;
            }
            time = time / 1440.0 + 0.00014;
            return time;
        }
        /// <summary>
        /// 过滤复制刀路
        /// </summary>
        /// <returns></returns>
        public List<OperationData> GetOperationFiltrationOrher(List<OperationData> data)
        {
            List<OperationData> newData = new List<OperationData>();
            foreach (OperationData od in data)
            {
                if (od.Oper.HasOtherInstances)
                {
                    bool isok = false;
                    foreach (NXOpen.CAM.Operation op in od.Oper.GetOtherInstances())
                    {
                        if (newData.Exists(a => a.Oper.Equals(op)))
                        {
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                        newData.Add(od);
                }
                else
                    newData.Add(od);
            }
            return newData;
        }
     
        private DateTime GetDateTimeBySeconds(double seconds)
        {
          
            return DateTime.FromOADate(seconds);
        }
    }
}
