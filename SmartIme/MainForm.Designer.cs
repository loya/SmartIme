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
        private System.Windows.Forms.TreeView treeApps;
        private System.Windows.Forms.Button btnAddApp;
        private System.Windows.Forms.Button btnRemoveApp;
        private System.Windows.Forms.ComboBox cmbDefaultIme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblCurrentIme = new Label();
            btnSwitchIme = new Button();
            treeApps = new TreeView();
            btnAddApp = new Button();
            btnRemoveApp = new Button();
            cmbDefaultIme = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            lblLog = new Label();
            btnExit = new Button();
            btnColorConfig = new Button();
            label3 = new Label();
            pnlCursorColor = new Panel();
            lblCursorColor = new Label();
            btnPickColor = new Button();
            cmbImeForColor = new ComboBox();
            label4 = new Label();
            colorDialog = new ColorDialog();
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
            btnSwitchIme.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSwitchIme.Location = new Point(200, 97);
            btnSwitchIme.Margin = new Padding(4);
            btnSwitchIme.Name = "btnSwitchIme";
            btnSwitchIme.Size = new Size(183, 42);
            btnSwitchIme.TabIndex = 1;
            btnSwitchIme.Text = "切换输入法";
            btnSwitchIme.UseVisualStyleBackColor = true;
            btnSwitchIme.Visible = false;
            // 
            // treeApps
            // 
            treeApps.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeApps.Font = new Font("微软雅黑", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            treeApps.Location = new Point(23, 145);
            treeApps.Margin = new Padding(4);
            treeApps.Name = "treeApps";
            treeApps.ShowNodeToolTips = true;
            treeApps.Size = new Size(569, 520);
            treeApps.TabIndex = 2;
            treeApps.DoubleClick += TreeApps_DoubleClick;
            // 
            // btnAddApp
            // 
            btnAddApp.Anchor = AnchorStyles.Bottom;
            btnAddApp.Location = new Point(171, 701);
            btnAddApp.Margin = new Padding(4);
            btnAddApp.Name = "btnAddApp";
            btnAddApp.Size = new Size(93, 44);
            btnAddApp.TabIndex = 3;
            btnAddApp.Text = "添加应用";
            btnAddApp.UseVisualStyleBackColor = true;
            // 
            // btnRemoveApp
            // 
            btnRemoveApp.Anchor = AnchorStyles.Bottom;
            btnRemoveApp.Location = new Point(272, 701);
            btnRemoveApp.Margin = new Padding(4);
            btnRemoveApp.Name = "btnRemoveApp";
            btnRemoveApp.Size = new Size(93, 44);
            btnRemoveApp.TabIndex = 4;
            btnRemoveApp.Text = "移除应用";
            btnRemoveApp.UseVisualStyleBackColor = true;
            // 
            // cmbDefaultIme
            // 
            cmbDefaultIme.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefaultIme.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDefaultIme.FormattingEnabled = true;
            cmbDefaultIme.Location = new Point(149, 68);
            cmbDefaultIme.Margin = new Padding(4);
            cmbDefaultIme.Name = "cmbDefaultIme";
            cmbDefaultIme.Size = new Size(443, 25);
            cmbDefaultIme.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(23, 107);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(129, 20);
            label1.TabIndex = 6;
            label1.Text = "应用输入法规则：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("微软雅黑", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(23, 68);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(114, 20);
            label2.TabIndex = 7;
            label2.Text = "默认输入法设置";
            // 
            // lblLog
            // 
            lblLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLog.AutoSize = true;
            lblLog.Location = new Point(23, 677);
            lblLog.Margin = new Padding(4, 0, 4, 0);
            lblLog.Name = "lblLog";
            lblLog.Size = new Size(43, 17);
            lblLog.TabIndex = 8;
            lblLog.Text = "label3";
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Bottom;
            btnExit.Location = new Point(373, 701);
            btnExit.Margin = new Padding(4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(93, 44);
            btnExit.TabIndex = 4;
            btnExit.Text = "退   出";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += BtnExit_Click;
            // 
            // btnColorConfig
            // 
            btnColorConfig.Anchor = AnchorStyles.Bottom;
            btnColorConfig.Location = new Point(474, 701);
            btnColorConfig.Margin = new Padding(4);
            btnColorConfig.Name = "btnColorConfig";
            btnColorConfig.Size = new Size(93, 44);
            btnColorConfig.TabIndex = 9;
            btnColorConfig.Text = "颜色配置";
            btnColorConfig.UseVisualStyleBackColor = true;
            btnColorConfig.Click += BtnColorConfig_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label3.Location = new Point(23, 750);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 10;
            label3.Text = "光标颜色:";
            // 
            // pnlCursorColor
            // 
            pnlCursorColor.BorderStyle = BorderStyle.FixedSingle;
            pnlCursorColor.Location = new Point(99, 748);
            pnlCursorColor.Margin = new Padding(4);
            pnlCursorColor.Name = "pnlCursorColor";
            pnlCursorColor.Size = new Size(20, 20);
            pnlCursorColor.TabIndex = 11;
            // 
            // lblCursorColor
            // 
            lblCursorColor.AutoSize = true;
            lblCursorColor.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblCursorColor.Location = new Point(127, 750);
            lblCursorColor.Margin = new Padding(4, 0, 4, 0);
            lblCursorColor.Name = "lblCursorColor";
            lblCursorColor.Size = new Size(43, 17);
            lblCursorColor.TabIndex = 12;
            lblCursorColor.Text = "黑色";
            // 
            // btnPickColor
            // 
            btnPickColor.Location = new Point(178, 748);
            btnPickColor.Margin = new Padding(4);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(75, 23);
            btnPickColor.TabIndex = 13;
            btnPickColor.Text = "选择颜色";
            btnPickColor.UseVisualStyleBackColor = true;
            btnPickColor.Click += BtnPickColor_Click;
            // 
            // cmbImeForColor
            // 
            cmbImeForColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImeForColor.FormattingEnabled = true;
            cmbImeForColor.Location = new Point(261, 748);
            cmbImeForColor.Margin = new Padding(4);
            cmbImeForColor.Name = "cmbImeForColor";
            cmbImeForColor.Size = new Size(140, 25);
            cmbImeForColor.TabIndex = 14;
            cmbImeForColor.SelectedIndexChanged += CmbImeForColor_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label4.Location = new Point(409, 750);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(68, 17);
            label4.TabIndex = 15;
            label4.Text = "输入法";
            // 
            // colorDialog
            // 
            colorDialog.AnyColor = true;
            colorDialog.FullOpen = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 780);
            Controls.Add(lblLog);
            Controls.Add(label4);
            Controls.Add(cmbImeForColor);
            Controls.Add(btnPickColor);
            Controls.Add(lblCursorColor);
            Controls.Add(pnlCursorColor);
            Controls.Add(label3);
            Controls.Add(btnColorConfig);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbDefaultIme);
            Controls.Add(btnExit);
            Controls.Add(btnRemoveApp);
            Controls.Add(btnAddApp);
            Controls.Add(treeApps);
            Controls.Add(btnSwitchIme);
            Controls.Add(lblCurrentIme);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "输入法智能切换助手";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblLog;
        private Button btnExit;
        private Button btnColorConfig;
        private Label label3;
        private Panel pnlCursorColor;
        private Label lblCursorColor;
        private Button btnPickColor;
        private ComboBox cmbImeForColor;
        private Label label4;
        private ColorDialog colorDialog;
    }
}