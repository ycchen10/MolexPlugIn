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
            try
            {
                theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_DIAMETER, out toolDia);
                this.ToolDia = toolDia;
            }
            catch
            {
                this.ToolDia = 0;
            }


            double toolLowerRad;
            try
            {
                theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_COR1_RAD, out toolLowerRad);
                this.ToolLowerRad = toolLowerRad;
            }
            catch
            {
                this.ToolLowerRad = 0;
            }

            double toolLength;
            try
            {
                theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_HEIGHT, out toolLength);
                this.ToolLength = toolLength;
            }
            catch
            {
                this.ToolLength = 0;
            }

            double fluteLength;
            try
            {
                theUFSession.Param.AskDoubleValue(np.Tag, UFConstants.UF_PARAM_TL_FLUTE_LN, out fluteLength);
                this.FluteLength = fluteLength;
            }
            catch
            {
                this.FluteLength = 0;
            }

            int toolNumber;
            try
            {
                theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_NUMBER, out toolNumber);
                this.ToolNumber = toolNumber;
            }
            catch
            {
                this.ToolNumber = 0;
            }

            int adiust;
            try
            {
                theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_ADJ_REG, out adiust);
                this.AdiustRegister = adiust;
            }
            catch
            {
                this.AdiustRegister = 0;
            }

            int cutcom;
            try
            {
                theUFSession.Param.AskIntValue(np.Tag, UFConstants.UF_PARAM_TL_CUTCOM_REG, out cutcom);
                this.CutcomRegister = cutcom;
            }
            catch
            {
                this.CutcomRegister = 0;
            }
        }

    }
}
