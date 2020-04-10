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


namespace MolexPlugin
{
    public partial class WorkpieceDrawingForm : Form
    {
        private AssembleModel assemble;
        public WorkpieceDrawingForm(AssembleModel assemble)
        {
            InitializeComponent();
            this.assemble = assemble;
            assemble.Works.Sort();
            foreach (WorkModel work in assemble.Works)
            {
                ListViewItem lv = new ListViewItem();
                lv.SubItems.Add("WORK" + work.WorkNumber.ToString());
                lv.SubItems.Add(work.WorkNumber.ToString());
                lv.Checked = true;
                listView.Items.Add(lv);
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
            WorkForWorkpieceDrawing dra;
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked)
                {
                    string workNum = listView.Items[i].SubItems[2].Text.ToString();
                    dra = new WorkForWorkpieceDrawing(this.assemble, int.Parse(workNum));
                    dra.CreateDrawing();
                }
            }
            PartUtils.SetPartDisplay(this.assemble.Asm.PartTag);
            this.Close();
        }
    }
}
