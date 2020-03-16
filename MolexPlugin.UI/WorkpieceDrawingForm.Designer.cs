namespace MolexPlugin
{
    partial class WorkpieceDrawingForm
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
            this.buttAllSelet = new System.Windows.Forms.Button();
            this.buttOk = new System.Windows.Forms.Button();
            this.buttCancel = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // buttAllSelet
            // 
            this.buttAllSelet.BackColor = System.Drawing.Color.White;
            this.buttAllSelet.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttAllSelet.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttAllSelet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttAllSelet.Location = new System.Drawing.Point(12, 157);
            this.buttAllSelet.Name = "buttAllSelet";
            this.buttAllSelet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttAllSelet.Size = new System.Drawing.Size(48, 34);
            this.buttAllSelet.TabIndex = 20;
            this.buttAllSelet.Text = "全选";
            this.buttAllSelet.UseVisualStyleBackColor = false;
            // 
            // buttOk
            // 
            this.buttOk.BackColor = System.Drawing.Color.White;
            this.buttOk.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttOk.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttOk.Location = new System.Drawing.Point(104, 157);
            this.buttOk.Name = "buttOk";
            this.buttOk.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttOk.Size = new System.Drawing.Size(48, 34);
            this.buttOk.TabIndex = 20;
            this.buttOk.Text = "确定";
            this.buttOk.UseVisualStyleBackColor = false;
            // 
            // buttCancel
            // 
            this.buttCancel.BackColor = System.Drawing.Color.White;
            this.buttCancel.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttCancel.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttCancel.Location = new System.Drawing.Point(192, 157);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttCancel.Size = new System.Drawing.Size(48, 34);
            this.buttCancel.TabIndex = 20;
            this.buttCancel.Text = "取消";
            this.buttCancel.UseVisualStyleBackColor = false;
            // 
            // listView
            // 
            this.listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader1});
            this.listView.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(12, 8);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(229, 143);
            this.listView.TabIndex = 21;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "选择";
            this.columnHeader.Width = 50;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Work";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 180;
            // 
            // WorkpieceDrawingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 203);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.buttOk);
            this.Controls.Add(this.buttAllSelet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkpieceDrawingForm";
            this.Text = "工件装夹图";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttAllSelet;
        private System.Windows.Forms.Button buttOk;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}