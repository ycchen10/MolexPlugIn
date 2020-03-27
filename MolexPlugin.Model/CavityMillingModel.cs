using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 面铣
    /// </summary>
    public class CavityMillingModel : AbstractOperationModel
    {
        private string templateOperName;
        public CavityMillingModel()
        {

        }
        public CavityMillingModel(NCGroupModel model, string templateName, string templateOperName) : base(model, templateName)
        {

        }
        public override void Create( string name)
        {
            base.CreateOperation( templateOperName, name, this.GroupModel);
        }

        public override OperationData GetOperationData(NXOpen.CAM.Operation oper)
        {
            OperationData data = base.GetOperationData(oper);
            NXOpen.CAM.CavityMillingBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(oper);
            if (operBuilder.CutParameters.FloorSameAsPartStock)
            {
                data.FloorStock = operBuilder.CutParameters.PartStock.Value;
                data.SideStock = operBuilder.CutParameters.PartStock.Value;
            }
            else
            {
                data.SideStock = operBuilder.CutParameters.PartStock.Value;
                data.FloorStock = operBuilder.CutParameters.FloorStock.Value;
            }
            data.Depth = operBuilder.CutLevel.GlobalDepthPerCut.DistanceBuilder.Value;
            if (operBuilder.BndStepover.DistanceBuilder.Intent == NXOpen.CAM.ParamValueIntent.ToolDep)
            {
                double toolDia = data.Tool.ToolDia;
                double dep = operBuilder.BndStepover.DistanceBuilder.Value;
                data.Stepover = toolDia * dep / 100;
            }
            else
            {
                data.Stepover = operBuilder.BndStepover.DistanceBuilder.Value;
            }

            data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
            data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;     
            operBuilder.Destroy();
            return data;
        }

        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            NXOpen.CAM.CavityMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
            List<Point> point = new List<Point>();
            foreach (Point3d p in pt)
            {
                point.Add(workPart.Points.CreatePoint(p));
            }
            builder1.NonCuttingBuilder.SetRegionStartPoints(point.ToArray());
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }

        public override void SetStock(double partStock, double floorStock)
        {
            NXOpen.CAM.CavityMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
            builder1.CutParameters.PartStock.Value = partStock;
            builder1.CutParameters.FloorStock.Value = floorStock;
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }
       
    }

}

