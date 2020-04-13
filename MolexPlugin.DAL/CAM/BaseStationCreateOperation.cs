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
        private List<BoundaryModel> conditions = new List<BoundaryModel>();
        public BaseStationCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfPlanarMilling(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (conditions.Count > 0)
                (this.Oper as PlanarMillingModel).SetBoundary(floorPt, conditions.ToArray());
            this.Oper.SetStock(-inter, 0.05);
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetBoundary(Point3d floorPt, params BoundaryModel[] conditions)
        {
            this.floorPt = floorPt;
            this.conditions = conditions.ToList();
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfBaseStation(program, this.ToolName);
        }
    }
}
