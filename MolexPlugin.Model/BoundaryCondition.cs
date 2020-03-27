using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 创建边界的条件
    /// </summary>
    public class BoundaryCondition
    {
        /// <summary>
        /// 边界
        /// </summary>
        public List<Edge> Edges { get; set; } = new List<Edge>();
        /// <summary>
        /// 边界最高点
        /// </summary>
        public Point3d BouudaryPt { get; set; }
        /// <summary>
        /// 边界开放还是封闭
        /// </summary>
        public BoundarySet.BoundaryTypes Types { get; set; }
        /// <summary>
        /// 刀具侧
        /// </summary>
        public BoundarySet.ToolSideTypes ToolSide { get; set; }

    }
}
