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
    public class SurfaceContourModel : AbstractOperationModel
    {
        private string templateOperName;
        private List<Face> faces;
        public SurfaceContourModel()
        {

        }
        public SurfaceContourModel(NCGroupModel model, string templateName, string templateOperName, List<Face> faces) : base(model, templateName)
        {
            this.templateOperName = templateOperName;
            this.faces = faces;
        }

        public override void Create(string name)
        {
            base.CreateOperation(this.templateOperName, name, this.GroupModel);
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.FeedsBuilder.SetMachiningData();
            NXOpen.CAM.GeometrySetList geometrySetList;
            geometrySetList = builder1.CutAreaGeometry.GeometryList;
            NXOpen.CAM.GeometrySet geometrySet2;
            geometrySet2 = builder1.CutAreaGeometry.CreateGeometrySet();
            geometrySetList.Append(geometrySet2);
            NXOpen.ScCollector scCollector1 = geometrySet2.ScCollector;
            ISelectionRule rule = new SelectionFaceRule(faces);
            SelectionIntentRule[] rules = new SelectionIntentRule[1] { rule.CreateSelectionRule() };
            scCollector1.ReplaceRules(rules, false);
            builder1.Commit();
            builder1.Destroy();
        }

        public override OperationData GetOperationData(NXOpen.CAM.Operation oper)
        {
            OperationData data = base.GetOperationData(oper);
            NXOpen.CAM.SurfaceContourBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(oper);
            data.FloorStock = operBuilder.CutParameters.PartStock.Value;
            data.SideStock = operBuilder.CutParameters.PartStock.Value;
            data.Depth = operBuilder.DmareaMillingBuilder.StepoverBuilder.DistanceBuilder.Value;
            data.Stepover = operBuilder.DmareaMillingBuilder.StepoverBuilder.DistanceBuilder.Value;
            data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
            data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;
            operBuilder.Destroy();
            return data;
        }

        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
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
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.CutParameters.PartStock.Value = partStock;
            builder1.CutParameters.FloorStock.Value = floorStock;
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }
        public void SetReferenceTool(Tool tool)
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.ReferenceTool = tool;
            builder1.FeedsBuilder.SetMachiningData();
            builder1.Commit();
            builder1.Destroy();
        }

    }

}

