using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using NXOpen;
using NXOpen.UF;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 获取刀具路径
    /// </summary>
    public class CreateOperationFactory
    {
        public static AbstractOperationModel GetOperation(NXOpen.CAM.Operation oper)
        {
            int type, subtype;
            AbstractOperationModel model = null;
            UFSession theUFSession = UFSession.GetUFSession();
            theUFSession.Obj.AskTypeAndSubtype(oper.Tag, out type, out subtype);
            switch (subtype)
            {
                case 260:
                    model = new CavityMillingModel(oper);
                    break;
                case 263:
                    model = new ZLevelMillingModel(oper);

                    break;
                case 210:
                    model = new SurfaceContourModel(oper);

                    break;
                case 110:
                    model = new PlanarMillingModel(oper);

                    break;
                case 261:
                    model = new FaceMillingModel(oper);

                    break;
                case 450:
                    model = new PointToPointModel(oper);
                    break;
                default:
                    break;
            }
            return model;
        }



    }
}
