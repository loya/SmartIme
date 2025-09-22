namespace SmartIme
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private System.Windows.Forms.Label lblCurrentIme;
        private System.Windows.Forms.Button btnSwitchIme;
        private System.Windows.Forms.ListBox lstApps;
        private System.Windows.Forms.Button btnAddApp;
        private System.Windows.Forms.Button btnRemoveApp;
        private System.Windows.Forms.ComboBox cmbDefaultIme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

        private void InitializeComponent()
        {
            lblCurrentIme = new Label();
            btnSwitchIme = new Button();
            lstApps = new ListBox();
            btnAddApp = new Button();
            btnRemoveApp = new Button();
            cmbDefaultIme = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            lblLog = new Label();
            btnExit = new Button();
            SuspendLayout();
            // 
            // lblCurrentIme
            // 
            lblCurrentIme.AutoSize = true;
            lblCurrentIme.Font = new Font("微软雅黑", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            lblCurrentIme.Location = new Point(23, 28);
            lblCurrentIme.Margin = new Padding(4, 0, 4, 0);
            lblCurrentIme.Name = "lblCurrentIme";
            lblCurrentIme.Size = new Size(106, 22);
            lblCurrentIme.TabIndex = 0;
            lblCurrentIme.Text = "当前输入法：";
            // 
            // btnSwitchIme
            // 
            btnSwitchIme.Location = new Point(23, 85);
            btnSwitchIme.Margin = new Padding(4);
            btnSwitchIme.Name = "btnSwitchIme";
            btnSwitchIme.Size = new Size(183, 42);
            btnSwitchIme.TabIndex = 1;
            btnSwitchIme.Text = "切换输入法";
            btnSwitchIme.UseVisualStyleBackColor = true;
            // 
            // lstApps
            // 
            lstApps.Font = new Font("微软雅黑", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lstApps.FormattingEnabled = true;
            lstApps.ItemHeight = 20;
            lstApps.Location = new Point(233, 136);
            lstApps.Margin = new Padding(4);
            lstApps.Name = "lstApps";
            lstApps.Size = new Size(360, 344);
            lstApps.TabIndex = 2;
            lstApps.DoubleClick += LstApps_DoubleClick;
            // 
            // btnAddApp
            // 
            btnAddApp.Location = new Point(133, 506);
            btnAddApp.Margin = new Padding(4);
            btnAddApp.Name = "btnAddApp";
            btnAddApp.Size = new Size(93, 42);
            btnAddApp.TabIndex = 3;
            btnAddApp.Text = "添加应用";
            btnAddApp.UseVisualStyleBackColor = true;
            // 
            // btnRemoveApp
            // 
            btnRemoveApp.Location = new Point(249, 506);
            btnRemoveApp.Margin = new Padding(4);
            btnRemoveApp.Name = "btnRemoveApp";
            btnRemoveApp.Size = new Size(93, 42);
            btnRemoveApp.TabIndex = 4;
            btnRemoveApp.Text = "移除应用";
            btnRemoveApp.UseVisualStyleBackColor = true;
            // 
            // cmbDefaultIme
            // 
            cmbDefaultIme.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDefaultIme.FormattingEnabled = true;
            cmbDefaultIme.Location = new Point(23, 170);
            cmbDefaultIme.Margin = new Padding(4);
            cmbDefaultIme.Name = "cmbDefaultIme";
            cmbDefaultIme.Size = new Size(182, 25);
            cmbDefaultIme.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(230, 96);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(105, 14);
            label1.TabIndex = 6;
            label1.Text = "应用输入法规则";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 142);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(92, 17);
            label2.TabIndex = 7;
            label2.Text = "默认输入法设置";
            // 
            // lblLog
            // 
            lblLog.AutoSize = true;
            lblLog.Location = new Point(26, 262);
            lblLog.Margin = new Padding(4, 0, 4, 0);
            lblLog.Name = "lblLog";
            lblLog.Size = new Size(43, 17);
            lblLog.TabIndex = 8;
            lblLog.Text = "label3";
            // 
            // btnExit
            // 
            btnExit.Location = new Point(360, 506);
            btnExit.Margin = new Padding(4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(93, 42);
            btnExit.TabIndex = 4;
            btnExit.Text = "退   出";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(608, 577);
            Controls.Add(lblLog);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbDefaultIme);
            Controls.Add(btnExit);
            Controls.Add(btnRemoveApp);
            Controls.Add(btnAddApp);
            Controls.Add(lstApps);
            Controls.Add(btnSwitchIme);
            Controls.Add(lblCurrentIme);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Margin = new Padding(4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "输入法智能切换助手";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblLog;
        private Button btnExit;
    }
}