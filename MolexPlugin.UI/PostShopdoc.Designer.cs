namespace MolexPlugin
{
    partial class PostShopdoc
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
            this.listViewProgram = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listBoxPostName = new System.Windows.Forms.ListBox();
            this.buttCancel = new System.Windows.Forms.Button();
            this.buttOk = new System.Windows.Forms.Button();
            this.buttAllSelet = new System.Windows.Forms.Button();
            this.buttonPost = new System.Windows.Forms.Button();
            this.buttonShopdoc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewProgram
            // 
            this.listViewProgram.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewProgram.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewProgram.CheckBoxes = true;
            this.listViewProgram.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader1});
            this.listViewProgram.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewProgram.FullRowSelect = true;
            this.listViewProgram.GridLines = true;
            this.listViewProgram.Location = new System.Drawing.Point(4, 12);
            this.listViewProgram.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listViewProgram.Name = "listViewProgram";
            this.listViewProgram.Size = new System.Drawing.Size(278, 391);
            this.listViewProgram.TabIndex = 26;
            this.listViewProgram.UseCompatibleStateImageBehavior = false;
            this.listViewProgram.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "选择";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "程序名";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 230;
            // 
            // listBoxPostName
            // 
            this.listBoxPostName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxPostName.FormattingEnabled = true;
            this.listBoxPostName.ItemHeight = 21;
            this.listBoxPostName.Items.AddRange(new object[] {
            "DAKIN",
            "HSM500",
            "MAKINO",
            "Mikron_Molex",
            "Electrode"});
            this.listBoxPostName.Location = new System.Drawing.Point(289, 12);
            this.listBoxPostName.Name = "listBoxPostName";
            this.listBoxPostName.Size = new System.Drawing.Size(169, 151);
            this.listBoxPostName.TabIndex = 27;
            // 
            // buttCancel
            // 
            this.buttCancel.BackColor = System.Drawing.Color.White;
            this.buttCancel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttCancel.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttCancel.Location = new System.Drawing.Point(380, 428);
            this.buttCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttCancel.Size = new System.Drawing.Size(74, 42);
            this.buttCancel.TabIndex = 28;
            this.buttCancel.Text = "取消";
            this.buttCancel.UseVisualStyleBackColor = false;
            this.buttCancel.Click += new System.EventHandler(this.buttCancel_Click);
            // 
            // buttOk
            // 
            this.buttOk.BackColor = System.Drawing.Color.White;
            this.buttOk.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttOk.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttOk.Location = new System.Drawing.Point(279, 428);
            this.buttOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttOk.Name = "buttOk";
            this.buttOk.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttOk.Size = new System.Drawing.Size(74, 42);
            this.buttOk.TabIndex = 29;
            this.buttOk.Text = "确定";
            this.buttOk.UseVisualStyleBackColor = false;
            this.buttOk.Click += new System.EventHandler(this.buttOk_Click);
            // 
            // buttAllSelet
            // 
            this.buttAllSelet.BackColor = System.Drawing.Color.White;
            this.buttAllSelet.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttAllSelet.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttAllSelet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttAllSelet.Location = new System.Drawing.Point(61, 428);
            this.buttAllSelet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttAllSelet.Name = "buttAllSelet";
            this.buttAllSelet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttAllSelet.Size = new System.Drawing.Size(135, 42);
            this.buttAllSelet.TabIndex = 30;
            this.buttAllSelet.Text = "全选";
            this.buttAllSelet.UseVisualStyleBackColor = false;
            this.buttAllSelet.Click += new System.EventHandler(this.buttAllSelet_Click);
            // 
            // buttonPost
            // 
            this.buttonPost.BackColor = System.Drawing.Color.White;
            this.buttonPost.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonPost.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttonPost.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPost.Location = new System.Drawing.Point(310, 231);
            this.buttonPost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonPost.Name = "buttonPost";
            this.buttonPost.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPost.Size = new System.Drawing.Size(135, 42);
            this.buttonPost.TabIndex = 30;
            this.buttonPost.Text = "后处理";
            this.buttonPost.UseVisualStyleBackColor = false;
            this.buttonPost.Click += new System.EventHandler(this.buttonPost_Click);
            // 
            // buttonShopdoc
            // 
            this.buttonShopdoc.BackColor = System.Drawing.Color.White;
            this.buttonShopdoc.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonShopdoc.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttonShopdoc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonShopdoc.Location = new System.Drawing.Point(310, 340);
            this.buttonShopdoc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonShopdoc.Name = "buttonShopdoc";
            this.buttonShopdoc.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonShopdoc.Size = new System.Drawing.Size(135, 42);
            this.buttonShopdoc.TabIndex = 30;
            this.buttonShopdoc.Text = "产生工单";
            this.buttonShopdoc.UseVisualStyleBackColor = false;
            this.buttonShopdoc.Click += new System.EventHandler(this.buttonShopdoc_Click);
            // 
            // PostShopdoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 482);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.buttOk);
            this.Controls.Add(this.buttonShopdoc);
            this.Controls.Add(this.buttonPost);
            this.Controls.Add(this.buttAllSelet);
            this.Controls.Add(this.listBoxPostName);
            this.Controls.Add(this.listViewProgram);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PostShopdoc";
            this.Text = "后处理";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewProgram;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListBox listBoxPostName;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.Button buttOk;
        private System.Windows.Forms.Button buttAllSelet;
        private System.Windows.Forms.Button buttonPost;
        private System.Windows.Forms.Button buttonShopdoc;
    }
}