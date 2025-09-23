namespace SmartIme
{
    partial class EditAppRulesForm
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
            lstRules = new ListBox();
            btnAddRule = new Button();
            btnRemoveRule = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // lstRules
            // 
            lstRules.FormattingEnabled = true;
            lstRules.ItemHeight = 17;
            lstRules.Location = new Point(14, 41);
            lstRules.Margin = new Padding(4, 4, 4, 4);
            lstRules.Name = "lstRules";
            lstRules.Size = new Size(419, 276);
            lstRules.TabIndex = 0;
            lstRules.DoubleClick += LstRules_DoubleClick;
            // 
            // btnAddRule
            // 
            btnAddRule.Location = new Point(14, 327);
            btnAddRule.Margin = new Padding(4, 4, 4, 4);
            btnAddRule.Name = "btnAddRule";
            btnAddRule.Size = new Size(88, 33);
            btnAddRule.TabIndex = 1;
            btnAddRule.Text = "添加规则";
            btnAddRule.UseVisualStyleBackColor = true;
            btnAddRule.Click += BtnAddRule_Click;
            // 
            // btnRemoveRule
            // 
            btnRemoveRule.Location = new Point(108, 327);
            btnRemoveRule.Margin = new Padding(4, 4, 4, 4);
            btnRemoveRule.Name = "btnRemoveRule";
            btnRemoveRule.Size = new Size(88, 33);
            btnRemoveRule.TabIndex = 2;
            btnRemoveRule.Text = "删除规则";
            btnRemoveRule.UseVisualStyleBackColor = true;
            btnRemoveRule.Click += BtnRemoveRule_Click;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(252, 327);
            btnOK.Margin = new Padding(4, 4, 4, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 33);
            btnOK.TabIndex = 3;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(346, 327);
            btnCancel.Margin = new Padding(4, 4, 4, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 33);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 13);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 5;
            label1.Text = "应用规则：";
            // 
            // EditAppRulesForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(448, 377);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(btnRemoveRule);
            Controls.Add(btnAddRule);
            Controls.Add(lstRules);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditAppRulesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "编辑应用规则";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox lstRules;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnRemoveRule;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
    }
}