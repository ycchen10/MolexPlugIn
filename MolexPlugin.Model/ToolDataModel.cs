using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 刀具
    /// </summary>
    public class ToolDataModel
    {
        /// <summary>
        /// 刀具
        /// </summary>
        public NXOpen.CAM.NCGroup ToolNcGroup { get; private set; }
        /// <summary>
        /// 刀具直径
        /// </summary>
        public double ToolDia { get; private set; }
        /// <summary>
        /// 刀具圆角半径
        /// </summary>
        public double ToolLowerRad { get; private set; }
        /// <summary>
        /// 刀具名
        /// </summary>
        public string ToolName { get; private set; }
        /// <summary>
        /// 刀具长度
        /// </summary>
        public double ToolLength { get; private set; }
        /// <summary>
        /// 刀刃长度
        /// </summary>
        public double FluteLength { get; private set; }
        /// <summary>
        /// 刀号
        /// </summary>
        public int ToolNumber { get; private set; }
        /// <summary>
        /// 刀具长度补偿
        /// </summary>
        public int AdiustRegister { get; private set; }
        /// <summary>
        /// 刀具半径补偿
        /// </summary>
        public int CutcomRegister { get; private set; }

        public ToolDataModel(NXOpen.CAM.NCGroup np)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            this.ToolNcGroup = np;
            this.ToolName = np.Name;

            double toolDia;
            theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_DIAMETER, out toolDia);
            this.ToolDia = toolDia;

            double toolLowerRad;
            theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_COR1_RAD, out toolLowerRad);
            this.ToolLowerRad = toolLowerRad;

            double toolLength;
            theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_HEIGHT, out toolLength);
            this.ToolLength = toolLength;

            double fluteLength;
            theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_FLUTE_LN, out fluteLength);
            this.FluteLength = fluteLength;

            int toolNumber;
            theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_NUMBER, out toolNumber);
            this.ToolNumber = toolNumber;

            int adiust;
            theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_ADJ_REG, out adiust);
            this.AdiustRegister = adiust;

            int cutcom;
            theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_CUTCOM_REG, out cutcom);
            this.CutcomRegister = cutcom;
        }

    }
}
