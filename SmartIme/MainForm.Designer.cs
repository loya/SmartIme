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
            this.lblCurrentIme = new System.Windows.Forms.Label();
            this.btnSwitchIme = new System.Windows.Forms.Button();
            this.lstApps = new System.Windows.Forms.ListBox();
            this.btnAddApp = new System.Windows.Forms.Button();
            this.btnRemoveApp = new System.Windows.Forms.Button();
            this.cmbDefaultIme = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCurrentIme
            // 
            this.lblCurrentIme.AutoSize = true;
            this.lblCurrentIme.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCurrentIme.Location = new System.Drawing.Point(20, 20);
            this.lblCurrentIme.Name = "lblCurrentIme";
            this.lblCurrentIme.Size = new System.Drawing.Size(106, 22);
            this.lblCurrentIme.TabIndex = 0;
            this.lblCurrentIme.Text = "当前输入法：";
            // 
            // btnSwitchIme
            // 
            this.btnSwitchIme.Location = new System.Drawing.Point(20, 60);
            this.btnSwitchIme.Name = "btnSwitchIme";
            this.btnSwitchIme.Size = new System.Drawing.Size(120, 30);
            this.btnSwitchIme.TabIndex = 1;
            this.btnSwitchIme.Text = "切换输入法";
            this.btnSwitchIme.UseVisualStyleBackColor = true;
            // 
            // lstApps
            // 
            this.lstApps.FormattingEnabled = true;
            this.lstApps.ItemHeight = 12;
            this.lstApps.Location = new System.Drawing.Point(200, 60);
            this.lstApps.Name = "lstApps";
            this.lstApps.Size = new System.Drawing.Size(180, 160);
            this.lstApps.TabIndex = 2;
            this.lstApps.DoubleClick += new System.EventHandler(this.lstApps_DoubleClick);
            // 
            // btnAddApp
            // 
            this.btnAddApp.Location = new System.Drawing.Point(200, 230);
            this.btnAddApp.Name = "btnAddApp";
            this.btnAddApp.Size = new System.Drawing.Size(80, 30);
            this.btnAddApp.TabIndex = 3;
            this.btnAddApp.Text = "添加应用";
            this.btnAddApp.UseVisualStyleBackColor = true;
            // 
            // btnRemoveApp
            // 
            this.btnRemoveApp.Location = new System.Drawing.Point(300, 230);
            this.btnRemoveApp.Name = "btnRemoveApp";
            this.btnRemoveApp.Size = new System.Drawing.Size(80, 30);
            this.btnRemoveApp.TabIndex = 4;
            this.btnRemoveApp.Text = "移除应用";
            this.btnRemoveApp.UseVisualStyleBackColor = true;
            // 
            // cmbDefaultIme
            // 
            this.cmbDefaultIme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultIme.FormattingEnabled = true;
            this.cmbDefaultIme.Location = new System.Drawing.Point(20, 120);
            this.cmbDefaultIme.Name = "cmbDefaultIme";
            this.cmbDefaultIme.Size = new System.Drawing.Size(120, 20);
            this.cmbDefaultIme.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(200, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "应用输入法规则";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "默认输入法设置";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 280);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbDefaultIme);
            this.Controls.Add(this.btnRemoveApp);
            this.Controls.Add(this.btnAddApp);
            this.Controls.Add(this.lstApps);
            this.Controls.Add(this.btnSwitchIme);
            this.Controls.Add(this.lblCurrentIme);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "输入法智能切换助手";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}