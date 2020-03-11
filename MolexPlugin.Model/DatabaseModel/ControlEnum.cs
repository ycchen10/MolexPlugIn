using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 控件下拉菜单
    /// </summary>
    public class ControlEnum
    {
        /// <summary>
        /// 控件ID
        /// </summary>
        public int ControlEnumId { get; set; }
        /// <summary>
        /// 下拉菜单名
        /// </summary>
        public string EnumName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ControlType { get; set; }

        public ControlEnum()
        {

        }
        public ControlEnum(string enumName, string controlType)
        {

            this.EnumName = enumName;
            this.ControlType = controlType;
        }
    }
}
