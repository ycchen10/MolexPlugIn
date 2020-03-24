namespace MolexPlugin
{
    partial class BomForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_OutExcel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_EditionNumber = new System.Windows.Forms.TextBox();
            this.textBox_PartNumber = new System.Windows.Forms.TextBox();
            this.textBox_MoldNumber = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.EleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EleX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EleY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EleZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PithX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PitchNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PitchY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PitchYNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CrudeInter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CrudeNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuringInter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DuringNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FineInter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FineNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EleType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.CH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Condition = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PreparationX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PreparationY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PreparationZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BorrowEle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(1112, 234);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(93, 37);
            this.button_Cancel.TabIndex = 11;
            this.button_Cancel.Text = "取消";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(937, 234);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(93, 37);
            this.button_OK.TabIndex = 12;
            this.button_OK.Text = "确定";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_OutExcel
            // 
            this.button_OutExcel.Location = new System.Drawing.Point(756, 234);
            this.button_OutExcel.Name = "button_OutExcel";
            this.button_OutExcel.Size = new System.Drawing.Size(93, 37);
            this.button_OutExcel.TabIndex = 13;
            this.button_OutExcel.Text = "导出BOM表";
            this.button_OutExcel.UseVisualStyleBackColor = true;
            this.button_OutExcel.Click += new System.EventHandler(this.button_OutExcel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(413, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "版本号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(215, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "件号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "模号：";
            // 
            // textBox_EditionNumber
            // 
            this.textBox_EditionNumber.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_EditionNumber.Location = new System.Drawing.Point(477, 248);
            this.textBox_EditionNumber.Name = "textBox_EditionNumber";
            this.textBox_EditionNumber.Size = new System.Drawing.Size(65, 23);
            this.textBox_EditionNumber.TabIndex = 5;
            // 
            // textBox_PartNumber
            // 
            this.textBox_PartNumber.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PartNumber.Location = new System.Drawing.Point(267, 248);
            this.textBox_PartNumber.Name = "textBox_PartNumber";
            this.textBox_PartNumber.Size = new System.Drawing.Size(126, 23);
            this.textBox_PartNumber.TabIndex = 6;
            // 
            // textBox_MoldNumber
            // 
            this.textBox_MoldNumber.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_MoldNumber.Location = new System.Drawing.Point(69, 248);
            this.textBox_MoldNumber.Name = "textBox_MoldNumber";
            this.textBox_MoldNumber.Size = new System.Drawing.Size(126, 23);
            this.textBox_MoldNumber.TabIndex = 7;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EleName,
            this.EleX,
            this.EleY,
            this.EleZ,
            this.PithX,
            this.PitchNumber,
            this.PitchY,
            this.PitchYNumber,
            this.CrudeInter,
            this.CrudeNumber,
            this.DuringInter,
            this.DuringNum,
            this.FineInter,
            this.FineNum,
            this.EleType,
            this.CH,
            this.Material,
            this.Condition,
            this.PreparationX,
            this.PreparationY,
            this.PreparationZ,
            this.BorrowEle});
            this.dataGridView.Location = new System.Drawing.Point(3, 8);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(1221, 216);
            this.dataGridView.TabIndex = 4;
            this.dataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView_CellBeginEdit);
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            // 
            // EleName
            // 
            this.EleName.DataPropertyName = "EleName";
            this.EleName.HeaderText = "电极名";
            this.EleName.Name = "EleName";
            this.EleName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.EleName.Width = 160;
            // 
            // EleX
            // 
            this.EleX.DataPropertyName = "EleSetValueX";
            this.EleX.HeaderText = "X";
            this.EleX.Name = "EleX";
            this.EleX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.EleX.Width = 70;
            // 
            // EleY
            // 
            this.EleY.DataPropertyName = "EleSetValueY";
            this.EleY.HeaderText = "Y";
            this.EleY.Name = "EleY";
            this.EleY.Width = 70;
            // 
            // EleZ
            // 
            this.EleZ.DataPropertyName = "EleSetValueZ";
            this.EleZ.HeaderText = "Z";
            this.EleZ.Name = "EleZ";
            this.EleZ.Width = 70;
            // 
            // PithX
            // 
            this.PithX.DataPropertyName = "PitchX";
            this.PithX.HeaderText = "PH-X";
            this.PithX.Name = "PithX";
            this.PithX.Width = 70;
            // 
            // PitchNumber
            // 
            this.PitchNumber.DataPropertyName = "PitchXNum";
            this.PitchNumber.HeaderText = "X个数";
            this.PitchNumber.Name = "PitchNumber";
            this.PitchNumber.Width = 70;
            // 
            // PitchY
            // 
            this.PitchY.DataPropertyName = "PitchY";
            this.PitchY.HeaderText = "PH-Y";
            this.PitchY.Name = "PitchY";
            this.PitchY.Width = 70;
            // 
            // PitchYNumber
            // 
            this.PitchYNumber.DataPropertyName = "PitchYNum";
            this.PitchYNumber.HeaderText = "Y个数";
            this.PitchYNumber.Name = "PitchYNumber";
            this.PitchYNumber.Width = 70;
            // 
            // CrudeInter
            // 
            this.CrudeInter.DataPropertyName = "CrudeInter";
            this.CrudeInter.HeaderText = "粗间隙";
            this.CrudeInter.Name = "CrudeInter";
            this.CrudeInter.Width = 70;
            // 
            // CrudeNumber
            // 
            this.CrudeNumber.DataPropertyName = "CrudeNum";
            this.CrudeNumber.HeaderText = "粗个数";
            this.CrudeNumber.Name = "CrudeNumber";
            this.CrudeNumber.Width = 70;
            // 
            // DuringInter
            // 
            this.DuringInter.DataPropertyName = "DuringInter";
            this.DuringInter.HeaderText = "中间隙";
            this.DuringInter.Name = "DuringInter";
            this.DuringInter.Width = 70;
            // 
            // DuringNum
            // 
            this.DuringNum.DataPropertyName = "DuringNum";
            this.DuringNum.HeaderText = "中个数";
            this.DuringNum.Name = "DuringNum";
            this.DuringNum.Width = 70;
            // 
            // FineInter
            // 
            this.FineInter.DataPropertyName = "FineInter";
            this.FineInter.HeaderText = "精间隙";
            this.FineInter.Name = "FineInter";
            this.FineInter.Width = 70;
            // 
            // FineNum
            // 
            this.FineNum.DataPropertyName = "FineNum";
            this.FineNum.HeaderText = "精个数";
            this.FineNum.Name = "FineNum";
            this.FineNum.Width = 70;
            // 
            // EleType
            // 
            this.EleType.DataPropertyName = "EleType";
            this.EleType.HeaderText = "电极类型";
            this.EleType.Name = "EleType";
            this.EleType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.EleType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EleType.Width = 70;
            // 
            // CH
            // 
            this.CH.DataPropertyName = "Ch";
            this.CH.HeaderText = "CH";
            this.CH.Name = "CH";
            this.CH.Width = 70;
            // 
            // Material
            // 
            this.Material.DataPropertyName = "Material";
            this.Material.HeaderText = "材料";
            this.Material.Name = "Material";
            this.Material.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Material.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Material.Width = 70;
            // 
            // Condition
            // 
            this.Condition.DataPropertyName = "Condition";
            this.Condition.HeaderText = "放电条件";
            this.Condition.Name = "Condition";
            this.Condition.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Condition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Condition.Width = 70;
            // 
            // PreparationX
            // 
            this.PreparationX.DataPropertyName = "PreparationX";
            this.PreparationX.HeaderText = "备料长";
            this.PreparationX.Name = "PreparationX";
            this.PreparationX.Width = 70;
            // 
            // PreparationY
            // 
            this.PreparationY.DataPropertyName = "PreparationY";
            this.PreparationY.HeaderText = "备料宽";
            this.PreparationY.Name = "PreparationY";
            this.PreparationY.Width = 70;
            // 
            // PreparationZ
            // 
            this.PreparationZ.DataPropertyName = "PreparationZ";
            this.PreparationZ.HeaderText = "备料高";
            this.PreparationZ.Name = "PreparationZ";
            this.PreparationZ.Width = 70;
            // 
            // BorrowEle
            // 
            this.BorrowEle.DataPropertyName = "BorrowName";
            this.BorrowEle.HeaderText = "借用电极";
            this.BorrowEle.Name = "BorrowEle";
            this.BorrowEle.Width = 160;
            // 
            // BomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 284);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_OutExcel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_EditionNumber);
            this.Controls.Add(this.textBox_PartNumber);
            this.Controls.Add(this.textBox_MoldNumber);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BomForm";
            this.Text = "BOM表";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_OutExcel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_EditionNumber;
        private System.Windows.Forms.TextBox textBox_PartNumber;
        private System.Windows.Forms.TextBox textBox_MoldNumber;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn EleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn EleX;
        private System.Windows.Forms.DataGridViewTextBoxColumn EleY;
        private System.Windows.Forms.DataGridViewTextBoxColumn EleZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn PithX;
        private System.Windows.Forms.DataGridViewTextBoxColumn PitchNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn PitchY;
        private System.Windows.Forms.DataGridViewTextBoxColumn PitchYNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn CrudeInter;
        private System.Windows.Forms.DataGridViewTextBoxColumn CrudeNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuringInter;
        private System.Windows.Forms.DataGridViewTextBoxColumn DuringNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn FineInter;
        private System.Windows.Forms.DataGridViewTextBoxColumn FineNum;
        private System.Windows.Forms.DataGridViewComboBoxColumn EleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn CH;
        private System.Windows.Forms.DataGridViewComboBoxColumn Material;
        private System.Windows.Forms.DataGridViewComboBoxColumn Condition;
        private System.Windows.Forms.DataGridViewTextBoxColumn PreparationX;
        private System.Windows.Forms.DataGridViewTextBoxColumn PreparationY;
        private System.Windows.Forms.DataGridViewTextBoxColumn PreparationZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn BorrowEle;
    }
}