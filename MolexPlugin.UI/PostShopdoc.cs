using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using System.IO;

namespace MolexPlugin
{
    public partial class PostShopdoc : Form
    {
        private ProgramNcGroupModel model;
        private NCGroup parent;
        private List<NCGroup> groups = new List<NCGroup>();
        public PostShopdoc()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (PostPartInfo.IsPartElectrode(workPart))
            {
                this.listBoxPostName.SelectedIndex = 4;
            }
            parent = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
            model = new ProgramNcGroupModel(parent);
            foreach (NCGroup np in model.GetProgram())
            {
                if (np.GetMembers().Length > 0)
                {
                    groups.Add(np);
                    ListViewItem lv = new ListViewItem();
                    lv.SubItems.Add(np.Name);
                    lv.Checked = true;
                    listViewProgram.Items.Add(lv);
                }
            }
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            if (buttonPost.Text.Equals("后处理"))
                buttonPost.Text = "不后处理";
            else
                buttonPost.Text = "后处理";
        }

        private void buttonShopdoc_Click(object sender, EventArgs e)
        {
            if (buttonShopdoc.Text.Equals("产生工单"))
                buttonShopdoc.Text = "不产生工单";
            else
                buttonShopdoc.Text = "产生工单";
        }

        private void buttAllSelet_Click(object sender, EventArgs e)
        {
            if (buttAllSelet.Text.Equals("全选"))
            {
                buttAllSelet.Text = "单选";
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    listViewProgram.Items[i].Checked = false;
                }
            }

            else
            {
                buttAllSelet.Text = "全选";
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    listViewProgram.Items[i].Checked = true;
                }

            }
        }

        private void buttOk_Click(object sender, EventArgs e)
        {
            Part workPart = Session.GetSession().Parts.Work;
            PartPost post = new PartPost(workPart);
            List<NCGroup> postGroup = new List<NCGroup>();
            if (buttonShopdoc.Text.Equals("产生工单"))
            {
                CreatePostExcel excel = new CreatePostExcel(groups, workPart);
                excel.CreateExcel();
            }
            if (buttonPost.Text == "后处理")
            {
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    if (listViewProgram.Items[i].Checked)
                    {
                        postGroup.Add(groups[i]);
                    }
                }

                if (this.listBoxPostName.SelectedItem.ToString().Equals("Electrode", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] name = post.GetElectrodePostName(groups);
                    foreach (string str in name)
                    {
                        post.Post(str, postGroup.ToArray());
                    }
                }
                else
                {
                    post.Post(this.listBoxPostName.SelectedItem.ToString(), postGroup.ToArray());
                }
            }

            this.Close();
        }

    }
}

