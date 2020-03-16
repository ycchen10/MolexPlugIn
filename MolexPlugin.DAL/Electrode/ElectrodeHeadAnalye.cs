using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    public class ElectrodeHeadAnalye
    {
        public static bool Analye(List<Body> bodys, Matrix4 workMat, string vecName)
        {
            Vector3d vec = new Vector3d();
            switch (vecName.ToUpper())
            {
                case "Z+":
                    vec = (new Vector3d(-workMat.GetZAxis().X, -workMat.GetZAxis().Y, -workMat.GetZAxis().Z));
                    break;
                case "X+":
                    vec = (new Vector3d(-workMat.GetXAxis().X, -workMat.GetXAxis().Y, -workMat.GetXAxis().Z));
                    break;
                case "X-":
                    vec = (workMat.GetXAxis());
                    break;
                case "Y+":
                    vec = (new Vector3d(-workMat.GetYAxis().X, -workMat.GetYAxis().Y, -workMat.GetYAxis().Z));
                    break;
                case "Y-":
                    vec = (workMat.GetZAxis());
                    break;
                default:
                    break;
            }
            bool isBack = false;
            foreach (Body body in bodys)
            {
                AnalyzeBuilder builder = new AnalyzeBuilder(body);
                builder.Analyze(vec);
                builder.GetBackOff();
                if (builder.IsBackOff)
                {
                    isBack = true;
                }
            }

            return isBack;
        }

    }
}
