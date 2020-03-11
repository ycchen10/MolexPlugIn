using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.Model
{

    public class AssembleModle
    {

        public AsmAssembleModel AsmModel { get;  set; } = null;

        public EDMAssembleModel EdmModel { get;  set; } = null;

        public List<ElectrodeAssembleModel> EleModel { get;  set; } = new List<ElectrodeAssembleModel>();

        public List<WorkAssembleModel> WorkModel { get;  set; } = new List<WorkAssembleModel>();

    }
}
