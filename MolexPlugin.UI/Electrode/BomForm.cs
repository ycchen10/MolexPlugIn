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
using NXOpen;

namespace MolexPlugin
{
    public partial class BomForm : Form
    {
        private AssembleModel assemble;
        private object oldValue;
        private DataGridViewTextBoxEditingControl CellEdit = null;
        private List<ElectrodeBomBuilder> builders = new List<ElectrodeBomBuilder>();
        private ElectrodeInfo oldInfo;
        public BomForm(AssembleModel assemble)
        {
            InitializeComponent();
            this.assemble = assemble;
            Initialize();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            foreach (ElectrodeBomBuilder bom in builders)
            {
                bom.Alter();
            }
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button_OutExcel_Click(object sender, EventArgs e)
        {
            OutPutBom.CreateBomExecl(assemble);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            this.textBox_MoldNumber.Text = assemble.Asm.MoldInfo.MoldNumber;
            this.textBox_PartNumber.Text = assemble.Asm.MoldInfo.WorkpieceNumber;
            this.textBox_EditionNumber.Text = assemble.Asm.MoldInfo.EditionNumber;
            EleType.Items.AddRange(GetContr("EleType").ToArray());
            Material.Items.AddRange(GetContr("Material").ToArray());
            Condition.Items.AddRange(GetContr("Condition").ToArray());
            dataGridView.Columns["EleX"].Visible = false; //隐藏列
            dataGridView.Columns["EleY"].Visible = false;
            dataGridView.Columns["EleZ"].Visible = false;
            dataGridView.Columns["EleName"].ReadOnly = true;  //只读列
            dataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige; //交替行不同颜色
            dataGridView.Columns[1].Frozen = true; //冻结首列
            dataGridView.AutoGenerateColumns = false;
            assemble.Electrodes.Sort();
            List<ElectrodeInfo> infos = new List<ElectrodeInfo>();
            foreach (ElectrodeModel model in assemble.Electrodes)
            {
                if (model.EleInfo.Positioning == "")
                    infos.Add(model.EleInfo);
            }
            DataTable table = ElectrodeInfo.GetDataTable(infos);
            dataGridView.DataSource = table;

            #region
            //foreach (ElectrodeModel ele in assemble.Electrodes)
            //{

            //    ElectrodeInfo info = ele.EleInfo;
            //    if (info.Positioning == "")
            //    {
            //        int index = dataGridView.Rows.Add();
            //        dataGridView.Rows[index].Cells[0].Value = info.EleName;
            //        dataGridView.Rows[index].Cells[1].Value = info.EleSetValue[0];
            //        dataGridView.Rows[index].Cells[2].Value = info.EleSetValue[1];
            //        dataGridView.Rows[index].Cells[3].Value = info.EleSetValue[2];

            //        dataGridView.Rows[index].Cells[4].Value = info.PitchX;
            //        dataGridView.Rows[index].Cells[5].Value = info.PitchXNum;
            //        dataGridView.Rows[index].Cells[6].Value = info.PitchY;
            //        dataGridView.Rows[index].Cells[7].Value = info.PitchYNum;


            //        dataGridView.Rows[index].Cells[8].Value = info.CrudeInter.ToString("f3");
            //        dataGridView.Rows[index].Cells[9].Value = info.CrudeNum;
            //        dataGridView.Rows[index].Cells[10].Value = info.DuringInter.ToString("f3");
            //        dataGridView.Rows[index].Cells[11].Value = info.DuringNum;
            //        dataGridView.Rows[index].Cells[12].Value = info.FineInter.ToString("f3");
            //        dataGridView.Rows[index].Cells[13].Value = info.FineNum;

            //        dataGridView.Rows[index].Cells[14].Value = info.EleType;

            //        dataGridView.Rows[index].Cells[15].Value = info.Ch;
            //        dataGridView.Rows[index].Cells[16].Value = info.Material;
            //        dataGridView.Rows[index].Cells[17].Value = info.Condition;

            //        dataGridView.Rows[index].Cells[18].Value = info.Preparation[0];
            //        dataGridView.Rows[index].Cells[19].Value = info.Preparation[1];
            //        dataGridView.Rows[index].Cells[20].Value = info.Preparation[2];

            //        dataGridView.Rows[index].Cells[21].Value = info.BorrowName;
            //    }
            //}
            #endregion
        }

        #region //控件过滤
        private void Cells_KeyPress1(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8 || e.KeyChar == '-')) // 过滤只能输入double
            {
                e.Handled = true;
            }
        }

        private void Cells_KeyPress2(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8)) // 过滤只能输入正double
            {
                e.Handled = true;
            }
        }

        private void Cells_KeyPress3(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 8)) // 过滤只能输入正整数
            {
                e.Handled = true;
            }
        }


        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (dataGridView.CurrentCellAddress.X == 4 || dataGridView.CurrentCellAddress.X == 6)
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
                CellEdit.KeyPress -= Cells_KeyPress2;
                CellEdit.KeyPress -= Cells_KeyPress3;
                CellEdit.KeyPress += Cells_KeyPress3; //过滤只能输入double
            }
            int[] column = { 5, 7, 9, 11, 13, 15, 18, 19, 20 };
            if (Array.IndexOf(column, dataGridView.CurrentCellAddress.X) != -1)
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
                CellEdit.KeyPress -= Cells_KeyPress2;
                CellEdit.KeyPress -= Cells_KeyPress3;
                CellEdit.KeyPress += Cells_KeyPress3; //过滤只能输入正整数
            }
            if (dataGridView.CurrentCellAddress.X == 8 || dataGridView.CurrentCellAddress.X == 10 || dataGridView.CurrentCellAddress.X == 12)
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
                CellEdit.KeyPress -= Cells_KeyPress2;
                CellEdit.KeyPress -= Cells_KeyPress3;
                CellEdit.KeyPress += Cells_KeyPress2; //绑定正double事件
            }
        }
        #endregion 
        /// <summary>
        /// 获取数据控件类型
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        private List<string> GetContr(string controlType)
        {
            List<string> control = new List<string>();
            var temp = ControlValue.Controls.GroupBy(a => a.ControlType);
            foreach (var i in temp)
            {
                if (i.Key == controlType)
                {
                    foreach (var k in i)
                    {
                        control.Add(k.EnumName);
                    }
                }
            }
            return control;
        }
        /// <summary>
        /// 开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = this.dataGridView.CurrentCell.Value;
            DataRow dr = (dataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
            oldInfo = ElectrodeInfo.GetEleInfo(dr);
        }
        /// <summary>
        /// 结束事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (oldValue != this.dataGridView.CurrentCell.Value)
            {
                ElectrodeBomBuilder bom;
                if (dataGridView.CurrentCellAddress.X == 4 || dataGridView.CurrentCellAddress.X == 5 || dataGridView.CurrentCellAddress.X == 6
                 || dataGridView.CurrentCellAddress.X == 7)
                {
                    double x = Convert.ToDouble(dataGridView.Rows[e.RowIndex].Cells[4].Value);
                    int xNumber = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[5].Value);
                    double y = Convert.ToDouble(dataGridView.Rows[e.RowIndex].Cells[6].Value);
                    int yNumber = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[7].Value);
                    string material = dataGridView.Rows[e.RowIndex].Cells[16].Value.ToString();

                    Point3d setPt = ElectrodeBomBuilder.GetSetValue(x, xNumber, y, yNumber, oldInfo);
                    dataGridView.Rows[e.RowIndex].Cells[1].Value = setPt.X.ToString("f3");
                    dataGridView.Rows[e.RowIndex].Cells[2].Value = setPt.Y.ToString("f3");

                    int[] pre = ElectrodeBomBuilder.GetPreparation(x, xNumber, y, yNumber, material, oldInfo);

                    dataGridView.Rows[e.RowIndex].Cells[18].Value = pre[0].ToString();
                    dataGridView.Rows[e.RowIndex].Cells[19].Value = pre[1].ToString();

                }

                DataRow dr = (dataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row; //获取数据
                ElectrodeInfo newInfo = ElectrodeInfo.GetEleInfo(dr);
                bom = new ElectrodeBomBuilder(newInfo, this.assemble);
                if (!builders.Exists(a => a.Model[0].EleInfo.EleName == newInfo.EleName))
                    this.builders.Add(bom);
                else
                {
                    ElectrodeBomBuilder bu = builders.Find(a => a.Model[0].EleInfo.EleName == newInfo.EleName);
                    this.builders.Remove(bu);
                    this.builders.Add(bom);
                }
            }
        }
    }
}
