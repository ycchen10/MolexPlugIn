using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using Basic;

namespace MolexPlugin
{
    public class WorkpieceDrawingCreateForm
    {

        private void ShowForm()
        {
            WorkpieceDrawingForm form = new WorkpieceDrawingForm();
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private void PartIsAsm()
        {
            Part workPart = Session.GetSession().Parts.Work;
            string type = AttributeUtils.GetAttrForString(workPart, "PartType");
            if(type.Equals("ASM"))
            {

            }
            else
            {

            }
        }
    }
}
