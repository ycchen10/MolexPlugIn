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
    public class FaceMillingModel : AbstractOperationModel
    {
        private string templateOperName;
        public FaceMillingModel(NXOpen.CAM.Operation oper) : base(oper)
        {

        }
        public FaceMillingModel(NCGroupModel model, string templateName, string templateOperName) : base(model, templateName)
        {
          
            this.templateOperName = templateOperName;
        }
        public override void Create(string name)
        {
            base.CreateOperation(templateOperName, name, this.GroupModel);           
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="conditions"></param>
        public void SetBoundary(params BoundaryModel[] conditions)
        {
            NXOpen.CAM.FaceMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateFaceMillingBuilder(this.Oper);
            builder1.FeedsBuilder.SetMachiningData();
            Boundary boundary = builder1.BlankBoundary;
            BoundarySetList list = boundary.BoundaryList;
            List<BoundaryMillingSet> boundarySet = new List<BoundaryMillingSet>();
            foreach (BoundaryModel bc in conditions)
            {
                boundarySet.Add(OperationUtils.CreateBoundaryMillingSet(bc.ToolSide, bc.Types,
              bc.BouudaryPt, boundary, bc.Edges.ToArray()));
            }
            list.Append(boundarySet.ToArray());
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = builder1.Commit();
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("PlanarMillingModel.SetBoundary 错误" + ex.Message);
            }
            finally
            {
                builder1.Destroy();
            }
        }
        public override OperationData GetOperationData()
        {
            OperationData data = base.GetOperationData();
            NXOpen.CAM.FaceMillingBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateFaceMillingBuilder(this.Oper);
            data.FloorStock = operBuilder.CutParameters.FloorStock.Value;
            data.SideStock = operBuilder.CutParameters.PartStock.Value;
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
            data.Depth = operBuilder.DepthPerCut.Value;
            data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
            data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;


            operBuilder.Destroy();
            return data;
        }

        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            NXOpen.CAM.FaceMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateFaceMillingBuilder(this.Oper);
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
            NXOpen.CAM.FaceMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateFaceMillingBuilder(this.Oper);
            builder1.CutParameters.PartStock.Value = partStock;
            builder1.CutParameters.FloorStock.Value = floorStock;
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }
        /// <summary>
        /// 设置毛坯距离
        /// </summary>
        /// <param name="dis"></param>
        public void SetBlankDistance(double dis)
        {
            NXOpen.CAM.FaceMillingBuilder builder1;
            builder1 = workPart.CAMSetup.CAMOperationCollection.CreateFaceMillingBuilder(this.Oper);
            builder1.CutParameters.BlankDistance.Value = dis;
            NXOpen.NXObject nXObject1;
            nXObject1 = builder1.Commit();
            builder1.Destroy();
        }
    }

}

