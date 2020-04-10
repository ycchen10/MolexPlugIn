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
        public SimplenessVerticalEleOperation(ElectrodeModel ele) : base(ele)
        {

        }
        public override void CreateOperation()
        {
           
        }

        public override void CreateOperationNameModel()
        {
            double min = analyze.GetMinDis();
            double minR = analyze.AnalyzeBody().MinRadius;
            int count = 1;
            if (min > 8.5 || min <= 6.5)
            {
                OperationNameModel name = ElectrodeCAMNameTemplate.AskOperationNameModelOfRough("O000" + count.ToString(), "EM8");
                this.NameModel.Add(name);
                count++;
            }
            if (min >= 6.5 && min <= 8.5)
            {
                OperationNameModel name = ElectrodeCAMNameTemplate.AskOperationNameModelOfRough("O000" + count.ToString(), "EM6");
                this.NameModel.Add(name);
                count++;
            }

            OperationNameModel name1 = ElectrodeCAMNameTemplate.AskOperationNameModelOfFaceMilling("O000" + count.ToString(), "EM2.98");
            this.NameModel.Add(name1);
            OperationNameModel name2 = ElectrodeCAMNameTemplate.AskOperationNameModelOfPlanarMilling("O000" + count.ToString(), "EM2.98");
            this.NameModel.Add(name2);
            count++;
            OperationNameModel name3 = ElectrodeCAMNameTemplate.AskOperationNameModelOfFaceMilling("O000" + count.ToString(), "EM2.98");
            this.NameModel.Add(name3);
            OperationNameModel name4 = ElectrodeCAMNameTemplate.AskOperationNameModelOfPlanarMilling("O000" + count.ToString(), "EM2.98");
            this.NameModel.Add(name4);
            count++;
            OperationNameModel name8 = ElectrodeCAMNameTemplate.AskOperationNameModelOfBaseStation("O000" + count.ToString(), "EM5.98");
            this.NameModel.Add(name3);
        }

    }
}
