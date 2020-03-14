using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using Basic;

namespace MolexPlugin
{
    public partial class EleStandardSeatForm : Form
    {
        private CreateConditionModel model;
        private CreateElectrodeBuilder builde;
        EletrodePreparation pre;

        private ElectrodeInfo eleInfo = new ElectrodeInfo();
        public EleStandardSeatForm(CreateConditionModel model)
        {
            InitializeComponent();
            this.model = model;
            builde = new CreateElectrodeBuilder(model);
            InitializeForm();
        }


        #region 过滤
        private void InputPlusInt(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 8)) // 过滤只能输入正整数
            {
                e.Handled = true;
            }
        }

        private void InputPlusDouble(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8)) // 过滤只能输入正double
            {
                e.Handled = true;
            }
        }

        private void InputDouble(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8 || e.KeyChar == '-')) // 过滤只能输入double
            {
                e.Handled = true;
            }
        }

        private void textBox_pitchX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_pitchY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_eleX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_eleY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_eleZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_pitchXNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_pitchYNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_crudeNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_duringNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_fineNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_CH_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_crudeInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void comboBox_duringInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void comboBox_fineInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_Ext_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }
        #endregion



        #region 预览
        private void textBox_pitchX_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }


        private void textBox_pitchXNum_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void textBox_pitchY_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void textBox_pitchYNum_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void button_X_Click(object sender, EventArgs e)
        {
            double temp = Convert.ToDouble(this.textBox_pitchX.Text);
            this.textBox_pitchX.Text = (-temp).ToString();
            CreatePreview();
        }

        private void button_Y_Click(object sender, EventArgs e)
        {
            double temp = Convert.ToDouble(this.textBox_pitchY.Text);
            this.textBox_pitchY.Text = (-temp).ToString();
            CreatePreview();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CreatePreview();
        }
        #endregion
        /// <summary>
        /// 初始化对话框
        /// </summary>
        private void InitializeForm()
        {
            comboBox_material.Items.AddRange(GetContr("Material").ToArray());
            comboBox_material.SelectedIndex = 0;
            comboBox_Condition.Items.AddRange(GetContr("Condition").ToArray());
            comboBox_Condition.SelectedIndex = 0;
            comboBox_eleType.Items.AddRange(GetContr("EleType").ToArray());
            comboBox_remarks.Items.AddRange(GetContr("Remarks").ToArray());
            comboBox_technology.Items.AddRange(GetContr("Technology").ToArray());
            comboBox_technology.SelectedIndex = 1;
            comboBox_cam.Items.AddRange(GetContr("Templates").ToArray());

            comboBox_crudeInter.Enabled = false;
            comboBox_crudeNum.Enabled = false;
            comboBox_duringInter.Enabled = false;
            comboBox_duringNum.Enabled = false;
            comboBox_fineInter.Text = "0.05";
            comboBox_fineNum.Text = "1";
            checkBox_fine.Checked = true;

            textBox_pitchYNum.Text = "1";
            textBox_pitchXNum.Text = "1";
            textBox_pitchX.Text = "0";
            textBox_pitchY.Text = "0";
            textBox_CH.Text = "18";
            textBox_Ext.Text = "1";
            SetPichContrShow();
            this.textBox_name.Text = builde.GetEleName();
            CreatePreview();
        }
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
        /// 预览
        /// </summary>
        private void CreatePreview()
        {
            double x = Convert.ToDouble(this.textBox_pitchX.Text);
            int xNumber = Convert.ToInt32(this.textBox_pitchXNum.Text);
            double y = Convert.ToDouble(this.textBox_pitchY.Text);
            int yNumber = Convert.ToInt32(this.textBox_pitchYNum.Text);
            builde.CreatePreveiw(x, xNumber, y, yNumber);
            Point3d value = builde.GetSetValue(Math.Abs(x), xNumber, Math.Abs(y), yNumber, checkBox1.Checked);
            double[] pre = builde.GetPreparation(x, xNumber, y, yNumber, checkBox1.Checked);


            this.textBox_eleX.Text = value.X.ToString();
            this.textBox_eleY.Text = value.Y.ToString();
            this.textBox_eleZ.Text = value.Z.ToString();

            this.textBox_preparationX.Text = pre[0].ToString();
            this.textBox_preparationY.Text = pre[1].ToString();
            this.textBox_preparationZ.Text = pre[2].ToString();
        }
        /// <summary>
        /// 设置PICH控件显示
        /// </summary>
        private void SetPichContrShow()
        {

            if (model.VecName.Equals("X+") || model.VecName.Equals("X-"))
            {
                this.textBox_pitchX.Enabled = false;
                this.textBox_pitchXNum.Enabled = false;
            }
            if (model.VecName.Equals("Y+") || model.VecName.Equals("Y-"))
            {
                this.textBox_pitchY.Enabled = false;
                this.textBox_pitchYNum.Enabled = false;
            }
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        private void GetEleInfo()
        {

            eleInfo.EleName = this.textBox_name.Text;

            eleInfo.PitchX = Math.Abs(double.Parse(this.textBox_pitchX.Text));
            eleInfo.PitchXNum = int.Parse(this.textBox_pitchXNum.Text);
            eleInfo.PitchY = Math.Abs(double.Parse(this.textBox_pitchY.Text));
            eleInfo.PitchYNum = int.Parse(this.textBox_pitchYNum.Text);

            eleInfo.EleSetValue[0] = double.Parse(this.textBox_eleX.Text);
            eleInfo.EleSetValue[1] = double.Parse(this.textBox_eleY.Text);
            eleInfo.EleSetValue[2] = double.Parse(this.textBox_eleZ.Text);

            eleInfo.Preparation[0] = int.Parse(this.textBox_preparationX.Text);
            eleInfo.Preparation[1] = int.Parse(this.textBox_preparationY.Text);
            eleInfo.Preparation[2] = int.Parse(this.textBox_preparationZ.Text);

            if (checkBox_crude.Checked)
            {
                string cru = this.comboBox_crudeInter.Text;
                string num = this.comboBox_crudeNum.Text;
                if (cru != "" && num != "")
                {
                    eleInfo.CrudeInter = double.Parse(cru);
                    eleInfo.CrudeNum = int.Parse(num);
                }
            }
            if (checkBox_during.Checked)
            {
                string cru = this.comboBox_duringInter.Text;
                string num = this.comboBox_duringNum.Text;
                if (cru != "" && num != "")
                {
                    eleInfo.DuringInter = double.Parse(cru);
                    eleInfo.DuringNum = int.Parse(num);
                }
            }
            if (checkBox_fine.Checked)
            {
                string cru = this.comboBox_fineInter.Text;
                string num = this.comboBox_fineNum.Text;
                if (cru != "" && num != "")
                {

                    eleInfo.FineInter = double.Parse(cru);
                    eleInfo.FineNum = int.Parse(num);
                }
            }
            eleInfo.Material = this.comboBox_material.Text;
            eleInfo.EleType = this.comboBox_eleType.Text;
            eleInfo.Condition = this.comboBox_Condition.Text;
            eleInfo.Extrudewith = double.Parse(this.textBox_Ext.Text);
            eleInfo.Ch = "CH" + this.textBox_CH.Text;

            string temp = "";
            if (this.checkBox_rotate.Checked)
                temp += "旋转电极";
            if (this.checkBox_clearEngle.Checked)
                temp += "清角电极";
            if (this.checkBox_wedm.Checked)
                temp += "线割电极";
            if (this.checkBox_clearGlitch.Checked)
                temp += "去毛刺电极";
            if (this.checkBox_gate.Checked)
                temp += "浇口电极";
            eleInfo.ElePresentation = temp;

            eleInfo.CamTemplate = this.comboBox_cam.Text;
            eleInfo.Technology = this.comboBox_technology.Text;
            eleInfo.Remarks = this.comboBox_remarks.Text;

            eleInfo.EleNumber = builde.GetEleNumber(this.textBox_name.Text);
            eleInfo.ZDatum = this.checkBox1.Checked;
        }

        private void buttOK_Click(object sender, EventArgs e)
        {
            builde.DelePreveiw();
            if (comboBox_eleType.Text == null || comboBox_eleType.Text == "")
            {
                UI.GetUI().NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "请选择电极类型！");
                return;
            }
            GetEleInfo();
            int x = int.Parse(this.textBox_preparationX.Text);
            int y = int.Parse(this.textBox_preparationY.Text);
            int z = int.Parse(this.textBox_preparationZ.Text);
            int[] tem = new int[2] { x, y };
            eleInfo.IsPreparation = pre.GetPreparation(ref tem);
            builde.CreateEle(eleInfo);
            this.Close();
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            builde.DelePreveiw();
            this.Close();
        }

        private void checkBox_crude_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_crude.Checked)
            {
                this.comboBox_crudeInter.Enabled = true;
                this.comboBox_crudeNum.Enabled = true;
            }
            else
            {
                this.comboBox_crudeInter.Enabled = false;
                this.comboBox_crudeNum.Enabled = false;
            }

        }

        private void checkBox_during_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_during.Checked)
            {
                this.comboBox_duringInter.Enabled = true;
                this.comboBox_duringNum.Enabled = true;
            }
            else
            {
                this.comboBox_duringInter.Enabled = false;
                this.comboBox_duringNum.Enabled = false;
            }

        }

        private void checkBox_fine_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_fine.Checked)
            {
                this.comboBox_fineInter.Enabled = true;
                this.comboBox_fineNum.Enabled = true;
            }
            else
            {
                this.comboBox_fineInter.Enabled = false;
                this.comboBox_fineNum.Enabled = false;
            }


        }

        private void comboBox_eleType_Leave(object sender, EventArgs e)
        {

            if (this.comboBox_material.Text.Equals("紫铜"))
            {
                pre = new EletrodePreparation("CuLength", "CuWidth");
            }
            else
            {
                pre = new EletrodePreparation("WuLength", "WuWidth");
            }
            int x = int.Parse(this.textBox_preparationX.Text);
            int y = int.Parse(this.textBox_preparationY.Text);
            int z = int.Parse(this.textBox_preparationZ.Text);
            int[] temp = new int[2] { x, y };
            pre.GetPreparation(ref temp);

            this.textBox_preparationX.Text = temp[0].ToString();
            this.textBox_preparationY.Text = temp[1].ToString();
        }


    }
}
