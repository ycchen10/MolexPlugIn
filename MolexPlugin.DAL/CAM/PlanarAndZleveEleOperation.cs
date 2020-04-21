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
    /// 直加等宽
    /// </summary>
    public class PlanarAndZleveEleOperation : AbstractElectrodeOperation
    {
        public PlanarAndZleveEleOperation(ElectrodeModel ele, ElectrodeCAMInfo info) : base(ele, info)
        {
            CreateOper();
        }

        private void CreateOper()
        {
            int count = 1;

            AbstractCreateOperation rough = new RoughCreateOperation(count, tool.GetRoughTool()); //开粗
            this.Oper.Add(rough);
            count++;
            string temp = tool.GetTwiceRoughTool();
            if (!temp.Equals(""))
            {
                TwiceRoughCreateOperation twice = new TwiceRoughCreateOperation(count, temp); //二次开粗
                twice.SetReferencetool(tool.GetRoughTool());
                this.Oper.Add(twice);
                count++;
            }
            FaceMillingCreateOperation face1 = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光平面           
            face1.SetBoundary(CamInfo.GetPlaneFaces().ToArray());
            this.Oper.Add(face1);
            PlanarMillingCreateOperation planar1 = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool());//光侧面
            planar1.SetBoundary(new Point3d(0, 0, this.CamInfo.BaseFace.BoxMinCorner.Z), this.CamInfo.BasePlanarPlanarBoundary.GetHoleBoundary().ToArray());
            this.Oper.Add(planar1);
            count++;

            ZLevelMillingCreateOperation zl = new ZLevelMillingCreateOperation(count, "BN0.98");
            zl.SetFaces(this.CamInfo.GetSteepFaces().ToArray());
            zl.SetCutLevel(Math.Abs(this.CamInfo.BaseFace.BoxMinCorner.Z - 0.05));
            this.Oper.Add(zl);
            count++;

            FaceMillingCreateOperation face2 = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光毛刺           
            face2.SetBoundary(CamInfo.GetPlaneFaces().ToArray());
            this.Oper.Add(face2);
            PlanarMillingCreateOperation planar2 = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool());//光毛刺
            planar2.SetBoundary(new Point3d(0, 0, this.CamInfo.BaseFace.BoxMinCorner.Z), this.CamInfo.BasePlanarPlanarBoundary.GetHoleBoundary().ToArray());
            planar2.SetBurringBool(true);
            this.Oper.Add(planar2);
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
