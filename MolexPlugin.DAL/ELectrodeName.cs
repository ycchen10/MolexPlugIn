using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using System.Text.RegularExpressions;

namespace MolexPlugin.DAL
{
    public class ElectrodeName
    {
        public static string GetEleName()
        {
            AssembleInstance assm = AssembleInstance.GetInstance();
            AssembleCollection colle = assm.GetAssembleModle();
            if (colle.Modle.EleModel.Count != 0)
            {
                colle.Modle.EleModel.Sort();
                string name = colle.Modle.EleModel[colle.Modle.EleModel.Count - 1].AssembleName;
                string eleName = name.Substring(0, name.LastIndexOf("E")+1);

                return eleName + (colle.Modle.EleModel[colle.Modle.EleModel.Count - 1].EleInfo.EleNumber + 1).ToString();
            }
            else
            {
                return colle.Modle.AsmModel.MoldInfo.MoldNumber + "-" + colle.Modle.AsmModel.MoldInfo.WorkpieceNumber + "E1";
            }

        }

        public static int GetEleNumber(string eleName)
        {
            string name = eleName.Substring(eleName.LastIndexOf("E"));

            MatchCollection match = Regex.Matches(name, @"\d+");
            int result;
            if (match.Count != 0 && int.TryParse(match[0].Value, out result))
            {
                return result;
            }
            return 1;
        }
    }
}
