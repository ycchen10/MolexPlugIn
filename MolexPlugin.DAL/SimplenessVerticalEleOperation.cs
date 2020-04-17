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
    /// 简单电极（没斜度）
    /// </summary>
    public class SimplenessVerticalEleOperation : AbstractElectrodeOperation
    {
        public SimplenessVerticalEleOperation(ElectrodeModel ele, bool isInter) : base(ele, isInter)
        {

        }
        public override void CreateOperation()
        {
            CreateOper();
        }

        public override void CreateOperationNameModel()
        {


        }

        private void CreateOper()
        {
            int count = 1;

            AbstractCreateOperation rough = new RoughCreateOperation(count, tool.GetRoughTool()); //开粗
            this.Oper.Add(rough);
            count++;
            string temp = tool.GetTwiceRoughTool();
            if (temp.Equals(""))
            {
                AbstractCreateOperation twice = new TwiceRoughCreateOperation(count, temp, tool.GetRoughTool()); //二次开粗
                this.Oper.Add(twice);
                count++;
            }
            AbstractCreateOperation face = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光平面
            this.Oper.Add(face);
            count++;
            AbstractCreateOperation planar = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool());//光侧面
            this.Oper.Add(planar);
            count++;
            AbstractCreateOperation station = new BaseStationCreateOperation(count, tool.GetBaseStationTool());//光基准台
            this.Oper.Add(station);

        }

    }
}
