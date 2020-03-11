using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodePreviewBuilder
    {

        private EleConditionModel model;

        public ElectrodePichModel pichModel { get; private set; }

        private ElectrodeHeadModel head;

        public ElectrodePreviewBuilder(EleConditionModel model, ElectrodeHeadModel head)
        {
            this.model = model;
            this.head = head;
            pichModel = new ElectrodePichModel(0, 1, 0, 1, head);
        }

        public void Preview(string x, string xNumber, string y, string yNumber)
        {

            pichModel.UpdatePattern(Convert.ToDouble(x), Convert.ToInt32(xNumber), Convert.ToDouble(y), Convert.ToInt32(yNumber));
        }

        public void DeleBuilder()
        {
            pichModel.DelePattern();
        }

    }
}
