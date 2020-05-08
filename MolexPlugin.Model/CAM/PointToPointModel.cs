using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;

namespace MolexPlugin.Model
{
    public class PointToPointModel : AbstractOperationModel
    {


        public PointToPointModel(NXOpen.CAM.Operation oper) : base(oper)
        {

        }
        /// <summary>
        /// 获取刀路数据
        /// </summary>
        public override OperationData GetOperationData()
        {
            OperationData data = base.GetOperationData();
            NXOpen.CAM.PointToPointBuilder operBuilder;
            operBuilder = workPart.CAMSetup.CAMOperationCollection.CreatePointToPointBuilder(this.Oper);
            data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
            data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;
            data.FloorStock = 0;
            data.SideStock = 0;
            data.Stepover = 0;
            return data;
        }



        public override void Create(string name)
        {
            throw new NotImplementedException();
        }

        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            throw new NotImplementedException();
        }

        public override void SetStock(double partStock, double floorStock)
        {
            throw new NotImplementedException();
        }
    }
}
