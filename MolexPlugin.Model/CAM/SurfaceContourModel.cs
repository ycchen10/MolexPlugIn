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
        public SurfaceContourModel(NXOpen.CAM.Operation oper) : base(oper)
        {

        }
        public SurfaceContourModel(NCGroupModel model, string templateName, string templateOperName) : base(model, templateName)
        {
            this.templateOperName = templateOperName;

        }

        public override void Create(string name)
        {
             base.CreateOperation(this.templateOperName, name, this.GroupModel);    
        }
      
        /// <summary>
        /// 设置加工面
        /// </summary>
        /// <param name="faces"></param>
        public void SetGeometry(params Face[] faces)
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.FeedsBuilder.SetMachiningData();
            NXOpen.CAM.GeometrySetList geometrySetList;
            geometrySetList = builder1.CutAreaGeometry.GeometryList;
            NXOpen.CAM.GeometrySet geometrySet2;
            geometrySet2 = builder1.CutAreaGeometry.CreateGeometrySet();
            geometrySetList.Append(geometrySet2);
            NXOpen.ScCollector scCollector1 = geometrySet2.ScCollector;
            ISelectionRule rule = new SelectionFaceRule(faces.ToList());
            SelectionIntentRule[] rules = new SelectionIntentRule[1] { rule.CreateSelectionRule() };
            scCollector1.ReplaceRules(rules, false);
            try
            {
                builder1.Commit();
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("SurfaceContourModel.SetGeometry 错误" + ex.Message);
            }
            finally
            {
                builder1.Destroy();
            }
        }
        public override OperationData GetOperationData()
        {
            OperationData data = base.GetOperationData();
            NXOpen.CAM.SurfaceContourBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
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
        /// <summary>
        /// 设置驱动
        /// </summary>
        /// <param name="types"></param>
        public void SetDriveMethod(NXOpen.CAM.SurfaceContourBuilder.DriveMethodTypes types)
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.SetDriveMethod(types);
            builder1.Commit();
            builder1.Destroy();
        }
        /// <summary>
        /// 设置清根参考刀
        /// </summary>
        /// <param name="tool"></param>
        public void SetReferenceTool(Tool tool)
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.ReferenceTool = tool;
            builder1.FeedsBuilder.SetMachiningData();
            builder1.Commit();
            builder1.Destroy();
        }
        /// <summary>
        /// 设置陡峭角
        /// </summary>
        public void SetSteep()
        {
            NXOpen.CAM.SurfaceContourBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateSurfaceContourBuilder(this.Oper);
            builder1.DmareaMillingBuilder.AmSteepOption = NXOpen.CAM.DmAmBuilder.SteepOptTypes.NonSteepNonDirectional;
            builder1.DmareaMillingBuilder.SteepAngle.Value = 55.0;
            builder1.Commit();
            builder1.Destroy();
        }
    }

}

