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
    /// 获取刀具路径数据
    /// </summary>
    public class OperationDataFactory
    {
        public static OperationData GetOperationData(NXOpen.CAM.Operation oper)
        {
            int type, subtype;
            AbstractOperationModel model;
            OperationData data = null;
            UFSession theUFSession = UFSession.GetUFSession();
            theUFSession.Obj.AskTypeAndSubtype(oper.Tag, out type, out subtype);
            switch (subtype)
            {
                case 260:
                    model = new CavityMillingModel(oper);
                    data = model.GetOperationData();
                    break;
                case 263:
                    model = new ZLevelMillingModel(oper);
                    data = model.GetOperationData();
                    break;
                case 210:
                    model = new SurfaceContourModel(oper);
                    data = model.GetOperationData();
                    break;
                case 110:
                    model = new PlanarMillingModel(oper);
                    data = model.GetOperationData();
                    break;
                case 261:
                    model = new FaceMillingModel(oper);
                    data = model.GetOperationData();
                    break;
                case 450:
                    PointToPointModel point = new PointToPointModel(oper);
                    data = point.GetOperationData();
                    break;
                default:
                    break;
            }
            return data;
        }



    }
}
