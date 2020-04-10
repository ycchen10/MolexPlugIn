using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.BlockStyler;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 创建刀路所需的名字
    /// </summary>
    public class OperationNameModel
    {
        /// <summary>
        /// 模板名
        /// </summary>
        public string templateName { get; set; }
        /// <summary>
        /// 刀路模板名
        /// </summary>
        public string templateOperName { get; set; }
        /// <summary>
        /// 刀路名
        /// </summary>
        public string OperName { get; set; }
        /// <summary>
        /// 刀具名
        /// </summary>
        public string ToolName { get; set; }
        /// <summary>
        /// 程序名
        /// </summary>
        public string ProgramName { get; set; }
        /// <summary>
        /// 图片名
        /// </summary>
        public string PngName { get; set; }
        /// <summary>
        ///刀路类型
        /// </summary>
        public OperationType OperType { get; set; }

        public Node Node { get; set; }
    }
    public enum OperationType
    {
        CavityMilling = 1,
        FaceMilling,
        PlanarMilling,
        PlanarMillingBase,
        PointToPoint,
        SurfaceContour,
        ZLevelMilling,
    }
}
