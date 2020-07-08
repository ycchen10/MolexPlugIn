using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace MolexPlugin.Model
{
    public class OperationData : IEquatable<OperationData>
    {
        /// <summary>
        /// 刀具路径
        /// </summary>
        public NXOpen.CAM.Operation Oper { get; set; }
        /// <summary>
        /// 过切检查
        /// </summary>
        public bool Check { get; set; }
        /// <summary>
        /// 刀路名
        /// </summary>
        public string OperName { get; set; }
        /// <summary>
        /// 程序组名
        /// </summary>
        public string OperGroup { get; set; }
        /// <summary>
        /// 刀路加工时间
        /// </summary>
        public double OperTime { get; set; }
        /// <summary>
        /// 刀具信息
        /// </summary>
        public ToolDataModel Tool { get; set; }
        /// <summary>
        /// 刀具组
        /// </summary>
        public NXOpen.CAM.NCGroup ToolNCGroup { get; set; }
        /// <summary>
        /// 刀路长度
        /// </summary>
        public double OprtLength { get; set; } = 0;
        /// <summary>
        /// 部件余量
        /// </summary>
        public double SideStock { get; set; } = 0;
        /// <summary>
        /// 底部余量
        /// </summary>
        public double FloorStock { get; set; } = 0;
        /// <summary>
        /// 进给
        /// </summary>
        public double Feed { get; set; } = 0;
        /// <summary>
        /// 转速
        /// </summary>
        public double Speed { get; set; } = 0;
        /// <summary>
        /// 部距
        /// </summary>
        public double Stepover { get; set; } = 0;
        /// <summary>
        /// 下刀量
        /// </summary>
        public double Depth { get; set; } = 0;
        /// <summary>
        /// Z向最大值
        /// </summary>
        public double Zmax { get; set; } = 0;
        /// <summary>
        /// Z向最小值
        /// </summary>
        public double Zmin { get; set; } = 0;
        /// <summary>
        /// 刀具半径补偿
        /// </summary>
        public string CutterCompenstation { get; set; } = "";
        /// <summary>
        /// 换刀号
        /// </summary>
        public string ToolNumber { get; set; }
        /// <summary>
        /// 刀长补
        /// </summary>
        public string ToolLengthNumber { get; set; }

        public bool Equals(OperationData other)
        {
            if (this.Tool.ToolName.Equals(other.Tool.ToolName, StringComparison.CurrentCultureIgnoreCase) &&
                Math.Round(this.OperTime, 7) == Math.Round(other.OperTime, 7) &&
                Math.Round(this.OprtLength, 7) == Math.Round(other.OprtLength, 7))
                return true;
            else
                return false;
        }
    }
}
