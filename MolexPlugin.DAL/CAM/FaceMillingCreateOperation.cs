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
        public List<Face> Conditions { get; private set; } = new List<Face>();
        public FaceMillingCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfFaceMilling(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (Conditions.Count != 0)
                (this.Oper as FaceMillingModel).SetBoundary(Conditions.ToArray());
            this.Oper.SetStock(0.05, -inter);
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetBoundary(params Face[] conditions)
        {
            this.Conditions = conditions.ToList();
        }

        private List<BoundaryModel> GetBoundary()
        {
            List<BoundaryModel> models = new List<BoundaryModel>();
            foreach (Face face in Conditions)
            {
                PlanarBoundary pb = new PlanarBoundary(face);
                BoundaryModel model1;
                double blank1;
                pb.GetPeripheralBoundary(out model1, out blank1);
                if (UMathUtils.IsEqual(blank1, 0))
                    models.Add(model1);
            }
            return models;
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfFaceMilling(program, this.ToolName);
        }

        public override AbstractCreateOperation CopyOperation()
        {
            FaceMillingCreateOperation fo = new FaceMillingCreateOperation(this.Site, this.ToolName);
            fo.CreateOperationName();
            // fo.SetBoundary(this.conditions.ToArray());
            return fo;
        }
    }
}
