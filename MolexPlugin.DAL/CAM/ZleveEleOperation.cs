﻿using System;
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
    ///等高
    /// </summary>
    public class ZleveEleOperation : AbstractElectrodeOperation
    {
        public ZleveEleOperation(ElectrodeModel ele, ElectrodeCAMInfo info) : base(ele, info)
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


            ZLevelMillingCreateOperation zl = new ZLevelMillingCreateOperation(count, "BN1.98");
            zl.SetFaces(this.CamInfo.GetAllFaces().ToArray());
            zl.SetCutLevel(this.CamInfo.BaseFace.Face);
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
