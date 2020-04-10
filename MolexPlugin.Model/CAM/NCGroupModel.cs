using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    public class NCGroupModel
    {
        /// <summary>
        /// 程序组
        /// </summary>
        public NCGroup ProgramGroup { get; set; }
        /// <summary>
        /// 加工方法
        /// </summary>
        public NCGroup MethodGroup { get; set; }
        /// <summary>
        /// 加工体
        /// </summary>
        public NCGroup GeometryGroup { get; set; }
        /// <summary>
        ///刀具数据
        /// </summary>
        public NCGroup ToolGroup { get; set; }


    }
}
