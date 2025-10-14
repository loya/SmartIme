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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            label1 = new Label();
            fontDialog = new FontDialog();
            btnOK = new Button();
            groupBox1 = new GroupBox();
            chkSameHintColor = new CheckBox();
            lblTextColor = new Label();
            label2 = new Label();
            pnlTextColorPreview = new Panel();
            btnBackColor = new Button();
            btnTextColor = new Button();
            lblBackColor = new Label();
            label4 = new Label();
            trackOpacity = new TrackBar();
            lblFontPreview = new Label();
            lblOpacityValue = new Label();
            btnFont = new Button();
            label3 = new Label();
            pnlBackColorPreview = new Panel();
            btnCancel = new Button();
            dgvHintColors = new DataGridView();
            panel1 = new Panel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackOpacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvHintColors).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(92, 17);
            label1.TabIndex = 3;
            label1.Text = "输入法提示颜色";
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom;
            btnOK.Location = new Point(137, 321);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 30);
            btnOK.TabIndex = 1;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += BtnOK_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkSameHintColor);
            groupBox1.Controls.Add(lblTextColor);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(pnlTextColorPreview);
            groupBox1.Controls.Add(btnBackColor);
            groupBox1.Controls.Add(btnTextColor);
            groupBox1.Controls.Add(lblBackColor);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(trackOpacity);
            groupBox1.Controls.Add(lblFontPreview);
            groupBox1.Controls.Add(lblOpacityValue);
            groupBox1.Controls.Add(btnFont);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(pnlBackColorPreview);
            groupBox1.Location = new Point(12, 100);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(5);
            groupBox1.Size = new Size(413, 207);
            groupBox1.TabIndex = 17;
            groupBox1.TabStop = false;
            groupBox1.Text = "提示窗设置";
            // 
            // chkSameHintColor
            // 
            chkSameHintColor.AutoSize = true;
            chkSameHintColor.Location = new Point(10, 170);
            chkSameHintColor.Name = "chkSameHintColor";
            chkSameHintColor.Size = new Size(135, 21);
            chkSameHintColor.TabIndex = 17;
            chkSameHintColor.Text = "跟随输入法提示颜色";
            chkSameHintColor.UseVisualStyleBackColor = true;
            // 
            // lblTextColor
            // 
            lblTextColor.AutoSize = true;
            lblTextColor.Location = new Point(233, 172);
            lblTextColor.Name = "lblTextColor";
            lblTextColor.Size = new Size(32, 17);
            lblTextColor.TabIndex = 16;
            lblTextColor.Text = "白色";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 36);
            label2.Name = "label2";
            label2.Size = new Size(80, 17);
            label2.TabIndex = 6;
            label2.Text = "背景色预览：";
            // 
            // pnlTextColorPreview
            // 
            pnlTextColorPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlTextColorPreview.Location = new Point(187, 173);
            pnlTextColorPreview.Name = "pnlTextColorPreview";
            pnlTextColorPreview.Size = new Size(16, 16);
            pnlTextColorPreview.TabIndex = 15;
            // 
            // btnBackColor
            // 
            btnBackColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBackColor.Location = new Point(332, 29);
            btnBackColor.Name = "btnBackColor";
            btnBackColor.Size = new Size(75, 30);
            btnBackColor.TabIndex = 4;
            btnBackColor.Text = "选择颜色";
            btnBackColor.UseVisualStyleBackColor = true;
            btnBackColor.Click += BtnBackColor_Click;
            // 
            // btnTextColor
            // 
            btnTextColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTextColor.Location = new Point(332, 165);
            btnTextColor.Name = "btnTextColor";
            btnTextColor.Size = new Size(75, 30);
            btnTextColor.TabIndex = 14;
            btnTextColor.Text = "选择颜色";
            btnTextColor.UseVisualStyleBackColor = true;
            btnTextColor.Click += BtnTextColor_Click;
            // 
            // lblBackColor
            // 
            lblBackColor.AutoSize = true;
            lblBackColor.Location = new Point(145, 36);
            lblBackColor.Name = "lblBackColor";
            lblBackColor.Size = new Size(58, 17);
            lblBackColor.TabIndex = 5;
            lblBackColor.Text = "#000000";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 127);
            label4.Name = "label4";
            label4.Size = new Size(80, 17);
            label4.TabIndex = 13;
            label4.Text = "提示窗字体：";
            // 
            // trackOpacity
            // 
            trackOpacity.Location = new Point(84, 66);
            trackOpacity.Maximum = 100;
            trackOpacity.Minimum = 10;
            trackOpacity.Name = "trackOpacity";
            trackOpacity.Size = new Size(252, 45);
            trackOpacity.TabIndex = 7;
            trackOpacity.Value = 60;
            trackOpacity.Scroll += TrackOpacity_Scroll;
            // 
            // lblFontPreview
            // 
            lblFontPreview.AutoSize = true;
            lblFontPreview.Location = new Point(93, 127);
            lblFontPreview.Name = "lblFontPreview";
            lblFontPreview.Size = new Size(89, 17);
            lblFontPreview.TabIndex = 12;
            lblFontPreview.Text = "微软雅黑, 10pt";
            // 
            // lblOpacityValue
            // 
            lblOpacityValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblOpacityValue.AutoSize = true;
            lblOpacityValue.Location = new Point(373, 82);
            lblOpacityValue.Name = "lblOpacityValue";
            lblOpacityValue.Size = new Size(33, 17);
            lblOpacityValue.TabIndex = 8;
            lblOpacityValue.Text = "60%";
            // 
            // btnFont
            // 
            btnFont.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFont.Location = new Point(332, 120);
            btnFont.Name = "btnFont";
            btnFont.Size = new Size(75, 30);
            btnFont.TabIndex = 11;
            btnFont.Text = "选择字体";
            btnFont.UseVisualStyleBackColor = true;
            btnFont.Click += BtnFont_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 82);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 9;
            label3.Text = "透明度：";
            // 
            // pnlBackColorPreview
            // 
            pnlBackColorPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlBackColorPreview.Location = new Point(94, 36);
            pnlBackColorPreview.Name = "pnlBackColorPreview";
            pnlBackColorPreview.Size = new Size(16, 16);
            pnlBackColorPreview.TabIndex = 10;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom;
            btnCancel.Location = new Point(218, 321);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // dgvHintColors
            // 
            dgvHintColors.AllowUserToAddRows = false;
            dgvHintColors.AllowUserToDeleteRows = false;
            dgvHintColors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvHintColors.BackgroundColor = SystemColors.InactiveBorder;
            dgvHintColors.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvHintColors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvHintColors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvHintColors.DefaultCellStyle = dataGridViewCellStyle2;
            dgvHintColors.Location = new Point(12, 3);
            dgvHintColors.Name = "dgvHintColors";
            dgvHintColors.RowHeadersVisible = false;
            dgvHintColors.ShowCellToolTips = false;
            dgvHintColors.Size = new Size(420, 86);
            dgvHintColors.TabIndex = 0;
            dgvHintColors.CellClick += DgvHintColors_CellClick;
            dgvHintColors.CellEndEdit += dgvHintColors_CellEndEdit;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(dgvHintColors);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(groupBox1);
            panel1.Controls.Add(btnOK);
            panel1.Location = new Point(0, 97);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(437, 363);
            panel1.TabIndex = 18;
            // 
            // HintColorSettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(437, 460);
            Controls.Add(label1);
            Controls.Add(panel1);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(450, 460);
            Name = "HintColorSettingsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "提示颜色设置";
            Load += HintColorSettingsForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackOpacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvHintColors).EndInit();
            panel1.ResumeLayout(false);
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
    private System.Windows.Forms.Button btnFont;
    private System.Windows.Forms.Label lblFontPreview;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnTextColor;
    private System.Windows.Forms.Panel pnlTextColorPreview;
    private System.Windows.Forms.Label lblTextColor;
    private System.Windows.Forms.FontDialog fontDialog;
    private GroupBox groupBox1;
        private Panel panel1;
        private CheckBox chkSameHintColor;
    }
}