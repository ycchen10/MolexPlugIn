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
    /// 简单电极（没斜度）
    /// </summary>
    public class SimplenessVerticalEleOperation : AbstractElectrodeOperation
    {
        public SimplenessVerticalEleOperation(ElectrodeModel ele, bool isInter) : base(ele, isInter)
        {
            CreateOper();
        }
        public override void CreateOperation(bool isInter)
        {
            PartUtils.SetPartDisplay(this.ele.PartTag);
            this.CreateCamSetup();
            this.SetWorkpiece();
            DeleteObject.UpdateObject();
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.CreateOperation(Cam, GetInter(isInter));
            }
        }

        public override void CreateOperationNameModel()
        {
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.CreateOperationName();
            }

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
                AbstractCreateOperation twice = new TwiceRoughCreateOperation(count, temp, tool.GetRoughTool()); //二次开粗
                this.Oper.Add(twice);
                count++;
            }
            FaceMillingCreateOperation face = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光平面
            List<BoundaryModel> models = new List<BoundaryModel>();
            foreach (PlanarBoundary pb in camInfo.GetPlaneFaces())
            {
                BoundaryModel model1;
                double blank1;
                pb.GetPeripheralBoundary(out model1, out blank1);
                if (UMathUtils.IsEqual(blank1, 0))
                    models.Add(model1);
            }
            face.SetBoundary(models.ToArray());
            this.Oper.Add(face);
            PlanarMillingCreateOperation planar = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool());//光侧面
            planar.SetBoundary(new Point3d(0, 0, camInfo.BaseFace.BoxMinCorner.Z), camInfo.BasePlanarPlanarBoundary.GetHoleBoundary().ToArray());
            this.Oper.Add(planar);
            count++;
            BaseStationCreateOperation station = new BaseStationCreateOperation(count, tool.GetBaseStationTool());//光基准台
            BoundaryModel model;
            double blank;
            camInfo.BasePlanarPlanarBoundary.GetPeripheralBoundary(out model, out blank);
            model.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.OutsideOrRight;
            station.SetBoundary(new Point3d(0, 0, camInfo.BaseSubfaceFace.BoxMinCorner.Z), model);
            this.Oper.Add(station);

        }

    }
}
