using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using Basic;
using NXOpen;

namespace MolexPlugin
{
    public partial class ElectrodeDrawingForm : Form
    {
        private AssembleModel assemble;
        public ElectrodeDrawingForm(AssembleModel assemble)
        {
            InitializeComponent();
            this.assemble = assemble;
            SetListView();
        }
        private void SetListView()
        {
            this.assemble.Works.Sort();
            foreach (WorkModel work in assemble.Works)
            {               
                List<ElectrodeModel> eles = this.assemble.Electrodes.Where(a => a.WorkNumber == work.WorkNumber).ToList();
                eles.Sort();
                foreach (ElectrodeModel em in eles)
                {
                    if (em.EleInfo.Positioning == "")
                    {
                        ListViewItem lv1 = new ListViewItem();
                        lv1.SubItems.Add(em.EleInfo.EleNumber.ToString());
                        lv1.SubItems.Add(em.EleInfo.EleName);
                        lv1.Checked = true;
                        listView.Items.Add(lv1);
                    }
                }
            }
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttAllSelet_Click(object sender, EventArgs e)
        {
            if (buttAllSelet.Text == "全选")
            {
                buttAllSelet.Text = "单选";

                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Checked = false;
                }
            }
            else
            {
                buttAllSelet.Text = "全选";

                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Checked = true;
                }
            }
        }

        private void buttOk_Click(object sender, EventArgs e)
        {
            ElectrodeDrawing dra;
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked)
                {
                    string workNum = listView.Items[i].SubItems[1].Text.ToString();
                    dra = new ElectrodeDrawing(this.assemble, int.Parse(workNum));
                    dra.CreateDrawing();
                    
                }
            }
            PartUtils.SetPartDisplay(this.assemble.Asm.PartTag);
            Session.GetSession().ApplicationSwitchImmediate("UG_APP_MODELING");
            this.Close();
        }
    }
}
