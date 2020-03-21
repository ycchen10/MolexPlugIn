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
    public class ElectrodeDrawingCreateForm
    {
        private AssembleModel assemble;
        private void ShowForm()
        {
            ElectrodeDrawingForm form = new ElectrodeDrawingForm(assemble);
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

                foreach (WorkModel work in assemble.Works)
                {
                    if (!AttributeUtils.GetAttrForBool(work.PartTag, "Interference"))
                    {
                        UI.GetUI().NXMessageBox.Show("提示", NXMessageBox.DialogType.Error, "WORK"+work.WorkNumber.ToString()
                            +"没有检查电极");
                        return false;
                    } 

                }
                if (assemble.IsAssmbleOk())
                    return true;
                else
                    return false;
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
