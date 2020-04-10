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
    /// 基准框光刀
    /// </summary>
    public class BaseStationCreateOperation : AbstractCreateOperation
    {
        private Point3d floorPt;
        private List<BoundaryModel> conditions;
        public BaseStationCreateOperation(int site, string tool, Point3d floorPt, params BoundaryModel[] conditions) : base(site, tool)
        {
            this.floorPt = floorPt;
            this.conditions = conditions.ToList();
        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfPlanarMilling(this.NameModel, eleCam);
            (this.Oper as PlanarMillingModel).SetBoundary(floorPt, conditions.ToArray());
            this.Oper.SetStock(-inter, 0.05);
        }

        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfBaseStation(program, this.ToolName);
        }
    }
}
