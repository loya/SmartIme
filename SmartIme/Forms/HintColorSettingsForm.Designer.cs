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
      DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
      DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
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
      btnFont = new Button();
      lblFontPreview = new Label();
      label4 = new Label();
      btnTextColor = new Button();
      pnlTextColorPreview = new Panel();
      lblTextColor = new Label();
      fontDialog = new FontDialog();
      groupBox1 = new GroupBox();
      ((System.ComponentModel.ISupportInitialize)dgvHintColors).BeginInit();
      ((System.ComponentModel.ISupportInitialize)trackOpacity).BeginInit();
      groupBox1.SuspendLayout();
      SuspendLayout();
      // 
      // dgvHintColors
      // 
      dgvHintColors.AllowUserToAddRows = false;
      dgvHintColors.AllowUserToDeleteRows = false;
      dgvHintColors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      dgvHintColors.BackgroundColor = SystemColors.InactiveBorder;
      dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle3.BackColor = SystemColors.Control;
      dataGridViewCellStyle3.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
      dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      dgvHintColors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
      dgvHintColors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle4.BackColor = SystemColors.Window;
      dataGridViewCellStyle4.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
      dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
      dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
      dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
      dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
      dgvHintColors.DefaultCellStyle = dataGridViewCellStyle4;
      dgvHintColors.Location = new Point(12, 35);
      dgvHintColors.Name = "dgvHintColors";
      dgvHintColors.ReadOnly = true;
      dgvHintColors.RowHeadersVisible = false;
      dgvHintColors.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
      dgvHintColors.Size = new Size(376, 117);
      dgvHintColors.TabIndex = 0;
      dgvHintColors.CellClick += DgvHintColors_CellClick;
      // 
      // btnOK
      // 
      btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      btnOK.Location = new Point(122, 417);
      btnOK.Name = "btnOK";
      btnOK.Size = new Size(75, 30);
      btnOK.TabIndex = 1;
      btnOK.Text = "确定";
      btnOK.UseVisualStyleBackColor = true;
      btnOK.Click += BtnOK_Click;
      // 
      // btnCancel
      // 
      btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      btnCancel.Location = new Point(203, 417);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(75, 30);
      btnCancel.TabIndex = 2;
      btnCancel.Text = "取消";
      btnCancel.UseVisualStyleBackColor = true;
      btnCancel.Click += BtnCancel_Click;
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
      // btnBackColor
      // 
      btnBackColor.Location = new Point(228, 27);
      btnBackColor.Name = "btnBackColor";
      btnBackColor.Size = new Size(75, 30);
      btnBackColor.TabIndex = 4;
      btnBackColor.Text = "选择颜色";
      btnBackColor.UseVisualStyleBackColor = true;
      btnBackColor.Click += BtnBackColor_Click;
      // 
      // lblBackColor
      // 
      lblBackColor.AutoSize = true;
      lblBackColor.Location = new Point(140, 34);
      lblBackColor.Name = "lblBackColor";
      lblBackColor.Size = new Size(58, 17);
      lblBackColor.TabIndex = 5;
      lblBackColor.Text = "#000000";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(6, 34);
      label2.Name = "label2";
      label2.Size = new Size(80, 17);
      label2.TabIndex = 6;
      label2.Text = "背景色预览：";
      // 
      // trackOpacity
      // 
      trackOpacity.Location = new Point(84, 75);
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
      lblOpacityValue.Location = new Point(328, 80);
      lblOpacityValue.Name = "lblOpacityValue";
      lblOpacityValue.Size = new Size(33, 17);
      lblOpacityValue.TabIndex = 8;
      lblOpacityValue.Text = "60%";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(6, 80);
      label3.Name = "label3";
      label3.Size = new Size(56, 17);
      label3.TabIndex = 9;
      label3.Text = "透明度：";
      // 
      // pnlBackColorPreview
      // 
      pnlBackColorPreview.BorderStyle = BorderStyle.FixedSingle;
      pnlBackColorPreview.Location = new Point(94, 34);
      pnlBackColorPreview.Name = "pnlBackColorPreview";
      pnlBackColorPreview.Size = new Size(16, 16);
      pnlBackColorPreview.TabIndex = 10;
      // 
      // btnFont
      // 
      btnFont.Location = new Point(286, 133);
      btnFont.Name = "btnFont";
      btnFont.Size = new Size(75, 30);
      btnFont.TabIndex = 11;
      btnFont.Text = "选择字体";
      btnFont.UseVisualStyleBackColor = true;
      btnFont.Click += BtnFont_Click;
      // 
      // lblFontPreview
      // 
      lblFontPreview.AutoSize = true;
      lblFontPreview.Location = new Point(91, 140);
      lblFontPreview.Name = "lblFontPreview";
      lblFontPreview.Size = new Size(89, 17);
      lblFontPreview.TabIndex = 12;
      lblFontPreview.Text = "微软雅黑, 10pt";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(6, 140);
      label4.Name = "label4";
      label4.Size = new Size(80, 17);
      label4.TabIndex = 13;
      label4.Text = "提示窗字体：";
      // 
      // btnTextColor
      // 
      btnTextColor.Location = new Point(286, 185);
      btnTextColor.Name = "btnTextColor";
      btnTextColor.Size = new Size(75, 30);
      btnTextColor.TabIndex = 14;
      btnTextColor.Text = "选择颜色";
      btnTextColor.UseVisualStyleBackColor = true;
      btnTextColor.Click += BtnTextColor_Click;
      // 
      // pnlTextColorPreview
      // 
      pnlTextColorPreview.BorderStyle = BorderStyle.FixedSingle;
      pnlTextColorPreview.Location = new Point(94, 192);
      pnlTextColorPreview.Name = "pnlTextColorPreview";
      pnlTextColorPreview.Size = new Size(16, 16);
      pnlTextColorPreview.TabIndex = 15;
      // 
      // lblTextColor
      // 
      lblTextColor.AutoSize = true;
      lblTextColor.Location = new Point(153, 192);
      lblTextColor.Name = "lblTextColor";
      lblTextColor.Size = new Size(32, 17);
      lblTextColor.TabIndex = 16;
      lblTextColor.Text = "白色";
      // 
      // groupBox1
      // 
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
      groupBox1.Location = new Point(12, 170);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new Size(376, 235);
      groupBox1.TabIndex = 17;
      groupBox1.TabStop = false;
      groupBox1.Text = "提示窗设置";
      // 
      // HintColorSettingsForm
      // 
      AutoScaleDimensions = new SizeF(7F, 17F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(400, 461);
      Controls.Add(label1);
      Controls.Add(btnCancel);
      Controls.Add(btnOK);
      Controls.Add(dgvHintColors);
      Controls.Add(groupBox1);
      Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "HintColorSettingsForm";
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "提示颜色设置";
      Load += HintColorSettingsForm_Load;
      ((System.ComponentModel.ISupportInitialize)dgvHintColors).EndInit();
      ((System.ComponentModel.ISupportInitialize)trackOpacity).EndInit();
      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
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
  }
}