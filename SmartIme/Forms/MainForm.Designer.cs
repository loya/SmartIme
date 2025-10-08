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
            pnlCursorColor = new Panel();
            lblCursorColor = new Label();
            btnPickColor = new Button();
            cmbImeForColor = new ComboBox();
            label4 = new Label();
            colorDialog = new ColorDialog();
            btnWhitelist = new Button();
            btnExpanAll = new Button();
            btnCollapseAll = new Button();
            btnRefresh = new Button();
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
            btnSwitchIme.Location = new Point(373, 13);
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
            treeApps.Font = new Font("微软雅黑", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            treeApps.HotTracking = true;
            treeApps.Location = new Point(23, 145);
            treeApps.Margin = new Padding(4);
            treeApps.Name = "treeApps";
            treeApps.ShowNodeToolTips = true;
            treeApps.Size = new Size(533, 278);
            treeApps.TabIndex = 2;
            treeApps.DoubleClick += TreeApps_DoubleClick;
            // 
            // btnAddApp
            // 
            btnAddApp.Anchor = AnchorStyles.Bottom;
            btnAddApp.Location = new Point(81, 524);
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
            btnRemoveApp.Location = new Point(189, 524);
            btnRemoveApp.Margin = new Padding(4);
            btnRemoveApp.Name = "btnRemoveApp";
            btnRemoveApp.Size = new Size(93, 44);
            btnRemoveApp.TabIndex = 4;
            btnRemoveApp.Text = "移除应用/规则";
            btnRemoveApp.UseVisualStyleBackColor = true;
            // 
            // cmbDefaultIme
            // 
            cmbDefaultIme.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefaultIme.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDefaultIme.FormattingEnabled = true;
            cmbDefaultIme.Location = new Point(159, 66);
            cmbDefaultIme.Margin = new Padding(4);
            cmbDefaultIme.Name = "cmbDefaultIme";
            cmbDefaultIme.Size = new Size(397, 25);
            cmbDefaultIme.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 11F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(23, 110);
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
            label2.Size = new Size(129, 20);
            label2.TabIndex = 7;
            label2.Text = "默认输入法设置：";
            // 
            // lblLog
            // 
            lblLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLog.AutoSize = true;
            lblLog.Location = new Point(23, 448);
            lblLog.Margin = new Padding(4, 0, 4, 0);
            lblLog.Name = "lblLog";
            lblLog.Size = new Size(43, 17);
            lblLog.TabIndex = 8;
            lblLog.Text = "label3";
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Bottom;
            btnExit.Location = new Point(405, 524);
            btnExit.Margin = new Padding(4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(93, 44);
            btnExit.TabIndex = 4;
            btnExit.Text = "退   出";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += BtnExit_Click;
            // 
            // pnlCursorColor
            // 
            pnlCursorColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pnlCursorColor.BorderStyle = BorderStyle.FixedSingle;
            pnlCursorColor.Location = new Point(336, 480);
            pnlCursorColor.Margin = new Padding(4);
            pnlCursorColor.Name = "pnlCursorColor";
            pnlCursorColor.Size = new Size(20, 20);
            pnlCursorColor.TabIndex = 11;
            // 
            // lblCursorColor
            // 
            lblCursorColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCursorColor.AutoSize = true;
            lblCursorColor.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblCursorColor.Location = new Point(369, 482);
            lblCursorColor.Margin = new Padding(4, 0, 4, 0);
            lblCursorColor.Name = "lblCursorColor";
            lblCursorColor.Size = new Size(32, 17);
            lblCursorColor.TabIndex = 12;
            lblCursorColor.Text = "黑色";
            // 
            // btnPickColor
            // 
            btnPickColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPickColor.AutoSize = true;
            btnPickColor.Location = new Point(233, 477);
            btnPickColor.Margin = new Padding(4);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(90, 27);
            btnPickColor.TabIndex = 13;
            btnPickColor.Text = "选择提示颜色";
            btnPickColor.UseVisualStyleBackColor = true;
            btnPickColor.Click += BtnPickColor_Click;
            // 
            // cmbImeForColor
            // 
            cmbImeForColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbImeForColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImeForColor.FormattingEnabled = true;
            cmbImeForColor.Location = new Point(80, 478);
            cmbImeForColor.Margin = new Padding(4);
            cmbImeForColor.Name = "cmbImeForColor";
            cmbImeForColor.Size = new Size(140, 25);
            cmbImeForColor.TabIndex = 14;
            cmbImeForColor.SelectedIndexChanged += CmbImeForColor_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label4.Location = new Point(23, 482);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(56, 17);
            label4.TabIndex = 15;
            label4.Text = "输入法：";
            // 
            // colorDialog
            // 
            colorDialog.AnyColor = true;
            colorDialog.FullOpen = true;
            // 
            // btnWhitelist
            // 
            btnWhitelist.Anchor = AnchorStyles.Bottom;
            btnWhitelist.Location = new Point(297, 524);
            btnWhitelist.Margin = new Padding(4);
            btnWhitelist.Name = "btnWhitelist";
            btnWhitelist.Size = new Size(93, 44);
            btnWhitelist.TabIndex = 16;
            btnWhitelist.Text = "白名单";
            btnWhitelist.UseVisualStyleBackColor = true;
            btnWhitelist.Click += BtnWhitelist_Click;
            // 
            // btnExpanAll
            // 
            btnExpanAll.AutoSize = true;
            btnExpanAll.FlatStyle = FlatStyle.Flat;
            btnExpanAll.Location = new Point(159, 106);
            btnExpanAll.Margin = new Padding(0);
            btnExpanAll.Name = "btnExpanAll";
            btnExpanAll.Size = new Size(84, 29);
            btnExpanAll.TabIndex = 17;
            btnExpanAll.Text = "▼ 展开所有";
            btnExpanAll.UseVisualStyleBackColor = true;
            btnExpanAll.Click += btnExpanAll_Click;
            // 
            // btnCollapseAll
            // 
            btnCollapseAll.AutoSize = true;
            btnCollapseAll.FlatStyle = FlatStyle.Flat;
            btnCollapseAll.Location = new Point(257, 106);
            btnCollapseAll.Margin = new Padding(0);
            btnCollapseAll.Name = "btnCollapseAll";
            btnCollapseAll.Size = new Size(84, 29);
            btnCollapseAll.TabIndex = 18;
            btnCollapseAll.Text = "▲ 折叠所有";
            btnCollapseAll.UseVisualStyleBackColor = true;
            btnCollapseAll.Click += BtnCollapseAll_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnRefresh.Location = new Point(516, 102);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(40, 30);
            btnRefresh.TabIndex = 19;
            btnRefresh.Text = "↻";
            btnRefresh.TextAlign = ContentAlignment.TopCenter;
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(579, 592);
            Controls.Add(btnRefresh);
            Controls.Add(btnCollapseAll);
            Controls.Add(btnExpanAll);
            Controls.Add(label4);
            Controls.Add(cmbImeForColor);
            Controls.Add(btnPickColor);
            Controls.Add(lblCursorColor);
            Controls.Add(pnlCursorColor);
            Controls.Add(lblLog);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbDefaultIme);
            Controls.Add(btnExit);
            Controls.Add(btnWhitelist);
            Controls.Add(btnRemoveApp);
            Controls.Add(btnAddApp);
            Controls.Add(treeApps);
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
        private Panel pnlCursorColor;
        private Label lblCursorColor;
        private Button btnPickColor;
        private ComboBox cmbImeForColor;
        private Label label4;
        private ColorDialog colorDialog;
        private Button btnWhitelist;
        private Button btnExpanAll;
        private Button btnCollapseAll;
        private Button btnRefresh;
    }
}