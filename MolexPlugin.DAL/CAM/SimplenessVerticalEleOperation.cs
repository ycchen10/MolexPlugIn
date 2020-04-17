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
        public SimplenessVerticalEleOperation(ElectrodeModel ele, ElectrodeCAMInfo info) : base(ele, info)
        {
            CreateOper();
        }
        public override void CreateOperation(bool isInter)
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (workPart.Tag != this.EleModel.PartTag.Tag)
                PartUtils.SetPartDisplay(this.EleModel.PartTag);
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
                TwiceRoughCreateOperation twice = new TwiceRoughCreateOperation(count, temp); //二次开粗
                twice.SetReferencetool(tool.GetTwiceRoughTool());
                this.Oper.Add(twice);
                count++;
            }
            FaceMillingCreateOperation face = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光平面           
            face.SetBoundary(CamInfo.GetPlaneFaces().ToArray());
            this.Oper.Add(face);
            PlanarMillingCreateOperation planar = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool());//光侧面
            planar.SetBoundary(new Point3d(0, 0, this.CamInfo.BaseFace.BoxMinCorner.Z), this.CamInfo.BasePlanarPlanarBoundary.GetHoleBoundary().ToArray());
            this.Oper.Add(planar);
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
