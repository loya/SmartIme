namespace SmartIme.Forms
{
  partial class HintColorSettingsForm
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
            dgvHintColors = new DataGridView();
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            btnBackColor = new Button();
            lblBackColor = new Label();
            label2 = new Label();
            trackOpacity = new TrackBar();
            lblOpacityValue = new Label();
            label3 = new Label();
            pnlBackColorPreview = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvHintColors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackOpacity).BeginInit();
            SuspendLayout();
            // 
            // dgvHintColors
            // 
            dgvHintColors.AllowUserToAddRows = false;
            dgvHintColors.AllowUserToDeleteRows = false;
            dgvHintColors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHintColors.BackgroundColor = SystemColors.ControlLight;
            dgvHintColors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHintColors.Location = new Point(12, 35);
            dgvHintColors.Name = "dgvHintColors";
            dgvHintColors.ReadOnly = true;
            dgvHintColors.RowHeadersVisible = false;
            dgvHintColors.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dgvHintColors.Size = new Size(376, 114);
            dgvHintColors.TabIndex = 0;
            dgvHintColors.CellClick += DgvHintColors_CellClick;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(232, 306);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 1;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(313, 306);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 16);
            label1.Name = "label1";
            label1.Size = new Size(92, 17);
            label1.TabIndex = 3;
            label1.Text = "输入法提示颜色";
            // 
            // btnBackColor
            // 
            btnBackColor.Location = new Point(207, 172);
            btnBackColor.Name = "btnBackColor";
            btnBackColor.Size = new Size(75, 23);
            btnBackColor.TabIndex = 4;
            btnBackColor.Text = "选择颜色";
            btnBackColor.UseVisualStyleBackColor = true;
            btnBackColor.Click += BtnBackColor_Click;
            // 
            // lblBackColor
            // 
            lblBackColor.AutoSize = true;
            lblBackColor.Location = new Point(152, 175);
            lblBackColor.Name = "lblBackColor";
            lblBackColor.Size = new Size(32, 17);
            lblBackColor.TabIndex = 5;
            lblBackColor.Text = "黑色";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 175);
            label2.Name = "label2";
            label2.Size = new Size(80, 17);
            label2.TabIndex = 6;
            label2.Text = "背景色预览：";
            // 
            // trackOpacity
            // 
            trackOpacity.Location = new Point(101, 211);
            trackOpacity.Maximum = 100;
            trackOpacity.Minimum = 10;
            trackOpacity.Name = "trackOpacity";
            trackOpacity.Size = new Size(234, 45);
            trackOpacity.TabIndex = 7;
            trackOpacity.Value = 60;
            trackOpacity.Scroll += TrackOpacity_Scroll;
            // 
            // lblOpacityValue
            // 
            lblOpacityValue.AutoSize = true;
            lblOpacityValue.Location = new Point(358, 211);
            lblOpacityValue.Name = "lblOpacityValue";
            lblOpacityValue.Size = new Size(33, 17);
            lblOpacityValue.TabIndex = 8;
            lblOpacityValue.Text = "60%";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 211);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 9;
            label3.Text = "透明度：";
            // 
            // pnlBackColorPreview
            // 
            pnlBackColorPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlBackColorPreview.Location = new Point(110, 175);
            pnlBackColorPreview.Name = "pnlBackColorPreview";
            pnlBackColorPreview.Size = new Size(16, 16);
            pnlBackColorPreview.TabIndex = 10;
            // 
            // HintColorSettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 336);
            Controls.Add(pnlBackColorPreview);
            Controls.Add(label3);
            Controls.Add(lblOpacityValue);
            Controls.Add(trackOpacity);
            Controls.Add(label2);
            Controls.Add(lblBackColor);
            Controls.Add(btnBackColor);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(dgvHintColors);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HintColorSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "提示颜色设置";
            Load += HintColorSettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvHintColors).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackOpacity).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHintColors;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnBackColor;
    private System.Windows.Forms.Label lblBackColor;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TrackBar trackOpacity;
    private System.Windows.Forms.Label lblOpacityValue;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Panel pnlBackColorPreview;
  }
}