namespace SmartIme
{
    partial class AddRuleForm
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
            lblName = new Label();
            txtName = new TextBox();
            lblType = new Label();
            radioProgram = new RadioButton();
            radioTitle = new RadioButton();
            radioControl = new RadioButton();
            lblContent = new Label();
            txtContent = new TextBox();
            lblMatchPattern = new Label();
            cmbMatchPattern = new ComboBox();
            lblIme = new Label();
            cmbIme = new ComboBox();
            btnSelectProcess = new Button();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(23, 28);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(68, 17);
            lblName.TabIndex = 0;
            lblName.Text = "规则名称：";
            // 
            // txtName
            // 
            txtName.Location = new Point(140, 28);
            txtName.Margin = new Padding(4);
            txtName.Name = "txtName";
            txtName.Size = new Size(291, 23);
            txtName.TabIndex = 1;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new Point(23, 71);
            lblType.Margin = new Padding(4, 0, 4, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(68, 17);
            lblType.TabIndex = 2;
            lblType.Text = "规则类型：";
            // 
            // radioProgram
            // 
            radioProgram.AutoSize = true;
            radioProgram.Checked = true;
            radioProgram.Location = new Point(140, 71);
            radioProgram.Margin = new Padding(4);
            radioProgram.Name = "radioProgram";
            radioProgram.Size = new Size(74, 21);
            radioProgram.TabIndex = 3;
            radioProgram.TabStop = true;
            radioProgram.Text = "程序名称";
            radioProgram.UseVisualStyleBackColor = true;
            // 
            // radioTitle
            // 
            radioTitle.AutoSize = true;
            radioTitle.Location = new Point(233, 71);
            radioTitle.Margin = new Padding(4);
            radioTitle.Name = "radioTitle";
            radioTitle.Size = new Size(74, 21);
            radioTitle.TabIndex = 4;
            radioTitle.Text = "窗口标题";
            radioTitle.UseVisualStyleBackColor = true;
            // 
            // radioControl
            // 
            radioControl.AutoSize = true;
            radioControl.Location = new Point(327, 71);
            radioControl.Margin = new Padding(4);
            radioControl.Name = "radioControl";
            radioControl.Size = new Size(74, 21);
            radioControl.TabIndex = 5;
            radioControl.Text = "控件类型";
            radioControl.UseVisualStyleBackColor = true;
            // 
            // lblContent
            // 
            lblContent.AutoSize = true;
            lblContent.Location = new Point(23, 113);
            lblContent.Margin = new Padding(4, 0, 4, 0);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(68, 17);
            lblContent.TabIndex = 4;
            lblContent.Text = "匹配内容：";
            // 
            // txtContent
            // 
            txtContent.Location = new Point(140, 113);
            txtContent.Margin = new Padding(4);
            txtContent.Name = "txtContent";
            txtContent.Size = new Size(291, 23);
            txtContent.TabIndex = 5;
            // 
            // lblMatchPattern
            // 
            lblMatchPattern.AutoSize = true;
            lblMatchPattern.Location = new Point(23, 156);
            lblMatchPattern.Margin = new Padding(4, 0, 4, 0);
            lblMatchPattern.Name = "lblMatchPattern";
            lblMatchPattern.Size = new Size(68, 17);
            lblMatchPattern.TabIndex = 6;
            lblMatchPattern.Text = "匹配模式：";
            // 
            // cmbMatchPattern
            // 
            cmbMatchPattern.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMatchPattern.FormattingEnabled = true;
            cmbMatchPattern.Location = new Point(140, 156);
            cmbMatchPattern.Margin = new Padding(4);
            cmbMatchPattern.Name = "cmbMatchPattern";
            cmbMatchPattern.Size = new Size(291, 25);
            cmbMatchPattern.TabIndex = 7;
            // 
            // lblIme
            // 
            lblIme.AutoSize = true;
            lblIme.Location = new Point(23, 199);
            lblIme.Margin = new Padding(4, 0, 4, 0);
            lblIme.Name = "lblIme";
            lblIme.Size = new Size(56, 17);
            lblIme.TabIndex = 8;
            lblIme.Text = "输入法：";
            // 
            // cmbIme
            // 
            cmbIme.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIme.FormattingEnabled = true;
            cmbIme.Location = new Point(140, 199);
            cmbIme.Margin = new Padding(4);
            cmbIme.Name = "cmbIme";
            cmbIme.Size = new Size(291, 25);
            cmbIme.TabIndex = 9;
            // 
            // btnSelectProcess
            // 
            btnSelectProcess.Location = new Point(140, 241);
            btnSelectProcess.Margin = new Padding(4);
            btnSelectProcess.Name = "btnSelectProcess";
            btnSelectProcess.Size = new Size(140, 42);
            btnSelectProcess.TabIndex = 10;
            btnSelectProcess.Text = "选择进程";
            btnSelectProcess.UseVisualStyleBackColor = true;
            btnSelectProcess.Visible = false;
            btnSelectProcess.Click += BtnSelectProcess_Click;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(140, 311);
            btnOk.Margin = new Padding(4);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(88, 42);
            btnOk.TabIndex = 11;
            btnOk.Text = "确定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += BtnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(257, 311);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 42);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddRuleForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(467, 382);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(btnSelectProcess);
            Controls.Add(cmbIme);
            Controls.Add(lblIme);
            Controls.Add(cmbMatchPattern);
            Controls.Add(lblMatchPattern);
            Controls.Add(txtContent);
            Controls.Add(lblContent);
            Controls.Add(radioControl);
            Controls.Add(radioTitle);
            Controls.Add(radioProgram);
            Controls.Add(lblType);
            Controls.Add(txtName);
            Controls.Add(lblName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddRuleForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "添加规则";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.RadioButton radioProgram;
        private System.Windows.Forms.RadioButton radioTitle;
        private System.Windows.Forms.RadioButton radioControl;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblMatchPattern;
        private System.Windows.Forms.ComboBox cmbMatchPattern;
        private System.Windows.Forms.Label lblIme;
        private System.Windows.Forms.ComboBox cmbIme;
        private System.Windows.Forms.Button btnSelectProcess;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}