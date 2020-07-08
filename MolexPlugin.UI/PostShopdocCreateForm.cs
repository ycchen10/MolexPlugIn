using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class PostShopdocCreateForm
    {
        private NCGroup gruop;
        private static void ShowForm()
        {
            PostShopdoc form = new PostShopdoc();
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private static bool PartIsAsm()
        {
            Session theSession = Session.GetSession();
            UI theUI = UI.GetUI();

            if (!theSession.ApplicationName.Equals("UG_APP_MANUFACTURING", StringComparison.CurrentCultureIgnoreCase))
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "请切换到加工模块");
                return false;
            }
            NCGroup group = GetNCGroup();
            if (group == null)
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "没法找到AAA程序组");
                return false;
            }
            ProgramNcGroupModel model = new ProgramNcGroupModel(group);
            if (!model.IsProgram())
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "程序组错误");
                return false;
            }
            foreach (NCGroup np in model.GetProgram())
            {
                ProgramNcGroupModel nc = new ProgramNcGroupModel(np);
                if (!nc.IsOperation() || !nc.IsOperationEqualTool())
                {
                    theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, np.Name + "错误");
                    return false;
                }
            }

            return true;
        }
        public static void Show()
        {
            if (PartIsAsm())
                ShowForm();
        }

        private static NXOpen.CAM.NCGroup GetNCGroup()
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.CAM.NCGroup parent = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
            return parent;
        }
    }
}
