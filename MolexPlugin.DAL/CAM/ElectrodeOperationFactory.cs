using MolexPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极程序工厂
    /// </summary>
    public class ElectrodeOperationFactory
    {
        public static AbstractElectrodeOperation CreateOperation(string template, ElectrodeModel ele, ElectrodeCAMInfo info)
        {
            AbstractElectrodeOperation ao = null;
            switch (template)
            {
                case "直电极":
                    ao = new SimplenessVerticalEleOperation(ele, info);
                    break;
                case "直+等宽":
                    ao = new PlanarAndSufaceEleOperation(ele, info);
                    break;
                case "直+等高":
                    ao = new PlanarAndZleveEleOperation(ele, info);
                    break;
                case "等宽+等高":
                    ao = new SufaceAndZleveEleOperation(ele, info);
                    break;
                case "等高电极":
                    ao = new ZleveEleOperation(ele, info);
                    break;
                case "User":
                    ao = new UserDefinedEleOperation(ele, info);
                    break;
                default:
                    break;
            }
            return ao;
        }
    }
}
