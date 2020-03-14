using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 设定值工厂
    /// </summary>
    class ElectrodeSetValueFactory
    {
        public static AbstractElectrodeSetValue Create(ElectrodeHeadInfo head, string dir)
        {
            AbstractElectrodeSetValue value = null;
            switch (dir.ToUpper())
            {
                case "Z+":
                    value = new ZPositiveElectrodeSetValue(head);
                    break;
                case "X+":
                    value = new XPositiveElectrodeSetValue(head);
                    break;
                case "X-":
                    value = new XNegativeElectrodeSetValue(head);
                    break;
                case "Y+":
                    value = new YPositiveElectrodeSetValue(head);
                    break;
                case "Y-":
                    value = new YNegativeElectrodeSetValue(head);
                    break;
                default:
                    break;
            }
            return value;

        }

    }
}
