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
    /// 光平面
    /// </summary>
    public class FaceMillingCreateOperation : AbstractCreateOperation
    {
        private List<BoundaryModel> conditions = new List<BoundaryModel>();
        public FaceMillingCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfFaceMilling(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (conditions.Count != 0)
                (this.Oper as FaceMillingModel).SetBoundary(conditions.ToArray());
            this.Oper.SetStock(0.05, -inter);
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetBoundary(params BoundaryModel[] conditions)
        {
            this.conditions = conditions.ToList();
        }

        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfFaceMilling(program, this.ToolName);
        }
    }
}
