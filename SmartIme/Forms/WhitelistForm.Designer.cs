namespace SmartIme
{
    partial class WhitelistForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            listWhitelist = new ListBox();
            btnAddApp = new Button();
            btnRemoveApp = new Button();
            btnClose = new Button();
            label1 = new Label();
            btnAddControl = new Button();
            SuspendLayout();
            // 
            // listWhitelist
            // 
            listWhitelist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listWhitelist.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            listWhitelist.FormattingEnabled = true;
            listWhitelist.ItemHeight = 19;
            listWhitelist.Location = new Point(14, 57);
            listWhitelist.Margin = new Padding(4);
            listWhitelist.Name = "listWhitelist";
            listWhitelist.Size = new Size(419, 327);
            listWhitelist.TabIndex = 0;
            listWhitelist.DoubleClick += listWhitelist_DoubleClick;
            // 
            // btnAddApp
            // 
            btnAddApp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddApp.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnAddApp.Location = new Point(45, 422);
            btnAddApp.Margin = new Padding(4);
            btnAddApp.Name = "btnAddApp";
            btnAddApp.Size = new Size(77, 39);
            btnAddApp.TabIndex = 1;
            btnAddApp.Text = "添加应用";
            btnAddApp.UseVisualStyleBackColor = true;
            btnAddApp.Click += BtnAddApp_Click;
            // 
            // btnRemoveApp
            // 
            btnRemoveApp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemoveApp.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnRemoveApp.Location = new Point(204, 422);
            btnRemoveApp.Margin = new Padding(4);
            btnRemoveApp.Name = "btnRemoveApp";
            btnRemoveApp.Size = new Size(77, 39);
            btnRemoveApp.TabIndex = 2;
            btnRemoveApp.Text = "移除应用";
            btnRemoveApp.UseVisualStyleBackColor = true;
            btnRemoveApp.Click += BtnRemoveApp_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnClose.Location = new Point(307, 422);
            btnClose.Margin = new Padding(4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(77, 39);
            btnClose.TabIndex = 3;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(14, 21);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(289, 20);
            label1.TabIndex = 4;
            label1.Text = "白名单中的应用切换前台时将不会悬浮提示：";
            // 
            // btnAddControl
            // 
            btnAddControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddControl.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnAddControl.Location = new Point(126, 422);
            btnAddControl.Margin = new Padding(4);
            btnAddControl.Name = "btnAddControl";
            btnAddControl.Size = new Size(77, 39);
            btnAddControl.TabIndex = 5;
            btnAddControl.Text = "添加控件";
            btnAddControl.UseVisualStyleBackColor = true;
            btnAddControl.Visible = false;
            // 
            // WhitelistForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(448, 483);
            Controls.Add(btnAddControl);
            Controls.Add(label1);
            Controls.Add(btnClose);
            Controls.Add(btnRemoveApp);
            Controls.Add(btnAddApp);
            Controls.Add(listWhitelist);
            Margin = new Padding(4);
            MinimumSize = new Size(347, 409);
            Name = "WhitelistForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "应用白名单";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listWhitelist;
        private System.Windows.Forms.Button btnAddApp;
        private System.Windows.Forms.Button btnRemoveApp;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private Button btnAddControl;
    }
}