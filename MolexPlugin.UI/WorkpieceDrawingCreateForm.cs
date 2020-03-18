using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class WorkpieceDrawingCreateForm
    {
        private AssembleModel assemble;
        private void ShowForm()
        {
            WorkpieceDrawingForm form = new WorkpieceDrawingForm(assemble);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private bool PartIsAsm()
        {
            Part workPart = Session.GetSession().Parts.Work;
            MoldInfoModel mold = new MoldInfoModel(workPart);
            string type = AttributeUtils.GetAttrForString(workPart, "PartType");
            if (type.Equals("ASM"))
            {
                assemble = AssembleSingleton.Instance().GetAssemble(mold.MoldNumber + "-" + mold.WorkpieceNumber);
                return true;
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("提示", NXMessageBox.DialogType.Error, "请切换到ASM下");
                return false;
            }
        }
        public void Show()
        {
            if (PartIsAsm())
                ShowForm();
        }
    }
}
