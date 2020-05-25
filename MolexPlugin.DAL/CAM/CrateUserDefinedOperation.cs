using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class CrateUserDefinedOperation
    {
        public static void Create()
        {
            Part workPart = Session.GetSession().Parts.Work;
            string type = AttributeUtils.GetAttrForString(workPart, "PartType");
            if (type.Equals("Electrode", StringComparison.CurrentCultureIgnoreCase))
            {
                ElectrodeModel model = new ElectrodeModel();
                model.GetModelForPart(workPart);
                CreateElectrodeCAM cam = new CreateElectrodeCAM(model, "User");
                cam.CreateOperName();
                cam.CreateOper();
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("注意", NXMessageBox.DialogType.Error, "电极不标准");
                return;
            }


        }
    }
}
