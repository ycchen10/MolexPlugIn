using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 用户自定义
    /// </summary>
    public class UserDefinedEleOperation : AbstractElectrodeOperation
    {
        public UserDefinedEleOperation(ElectrodeModel ele, ElectrodeCAMInfo info) : base(ele, info)
        {
            CreateOper();
        }

        private void CreateOper()
        {
            int count = 1;

            AbstractCreateOperation rough = new RoughCreateOperation(count, "EM8"); //开粗
            this.Oper.Add(rough);
            count++;

            TwiceRoughCreateOperation twice = new TwiceRoughCreateOperation(count, "EM3"); //二次开粗
            twice.SetReferencetool("EM8");
            this.Oper.Add(twice);
            count++;

            FaceMillingCreateOperation face1 = new FaceMillingCreateOperation(count, "EM2.98"); //光平面                   
            this.Oper.Add(face1);
            PlanarMillingCreateOperation planar1 = new PlanarMillingCreateOperation(count, "EM2.98");//光侧面           
            this.Oper.Add(planar1);
            count++;

            SurfaceContourCreateOperation so = new SurfaceContourCreateOperation(count, "BN1.98");         
            this.Oper.Add(so);
            ZLevelMillingCreateOperation zl = new ZLevelMillingCreateOperation(count, "BN1.98"); 
            this.Oper.Add(zl);
            count++;


            BaseStationCreateOperation station = new BaseStationCreateOperation(count, tool.GetBaseStationTool());//光基准台
            BoundaryModel model;
            double blank;
            this.CamInfo.BasePlanarPlanarBoundary.GetPeripheralBoundary(out model, out blank);
            model.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.OutsideOrRight;
            station.SetBoundary(new Point3d(0, 0, this.CamInfo.BaseSubfaceFace.BoxMinCorner.Z), model);
            this.Oper.Add(station);

        }

    }
}
