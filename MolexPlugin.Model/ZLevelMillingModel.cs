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
    public class ZLevelMillingModel : AbstractOperationModel
    {
        private string templateOperName;
        private List<Face> faces;
        public ZLevelMillingModel()
        {

        }
        public ZLevelMillingModel(NCGroupModel model, string templateName, string templateOperName, List<Face> faces) : base(model, templateName)
        {
            this.templateOperName = templateOperName;
            this.faces = faces;
        }

        public override void Create(string name)
        {
            base.CreateOperation(this.templateOperName, name, this.GroupModel);
            NXOpen.CAM.ZLevelMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateZlevelMillingBuilder(this.Oper);
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
            builder1.Destroy();
        }

        public override OperationData GetOperationData(NXOpen.CAM.Operation oper)
        {
            OperationData data = base.GetOperationData(oper);
            NXOpen.CAM.ZLevelMillingBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateZlevelMillingBuilder(oper);          
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
            if (operBuilder.CutParameters.CutBetweenLevels)
            {
                data.Stepover = operBuilder.CutParameters.Stepover.DistanceBuilder.Value;
            }
            else
            {
                data.Stepover = 0;
            }

            data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
            data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;
            operBuilder.Destroy();
            return data;
        }

        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            NXOpen.CAM.ZLevelMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateZlevelMillingBuilder(this.Oper);
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
            NXOpen.CAM.ZLevelMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateZlevelMillingBuilder(this.Oper);
            builder1.CutParameters.PartStock.Value = partStock;
            builder1.CutParameters.FloorStock.Value = floorStock;
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }
        /// <summary>
        /// 设置切削层
        /// </summary>
        /// <param name="zLevel"></param>
        public void SetCutLevel(double zLevel)
        {
            NXOpen.CAM.ZLevelMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateZlevelMillingBuilder(this.Oper);
            builder1.CutLevel.SetRangeDepth(0, zLevel, NXOpen.CAM.CutLevel.MeasureTypes.TopLevel);
            builder1.Destroy();
        }
    }

}

