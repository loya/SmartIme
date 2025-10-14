using SmartIme.Models;
using SmartIme.Utilities;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SmartIme.Forms
{
    public partial class HintColorSettingsForm : Form
    {
        private ColorDialog _colorDialog;
        // private PictureBox picChinesePreview;
        // private PictureBox picEnglishPreview;
        private List<PictureBox> picPreviews = new List<PictureBox>();
        private AppSettings _appSettings;
        private BindingList<ImeColorItem> _imeColorItems;

        public bool TexColorSameHintColor
        {
            get => _appSettings.TextColorSameHintColor;
            set
            {
                _appSettings.TextColorSameHintColor = value;
                if (chkSameHintColor != null)
                    chkSameHintColor.Checked = value;
                UpdatePreviewImages();
            }
        }

        public List<ImeColor> ImeColors
        {
            get => _appSettings.ImeColors;
            set
            {
                _appSettings.ImeColors = value;
                // 如果_imeColorItems已经初始化，立即加载颜色设置
                // if (_imeColorItems != null && _imeColorItems.Count > 0)
                // {
                //     LoadColorSettings();
                // }
            }
        }

        public Color HintBackColor
        {
            get => _appSettings.HintBackColor.GetValueOrDefault(Color.Black);
            set
            {
                _appSettings.HintBackColor = value;
                UpdatePreviewImages();

            }
        }

        public double HintOpacity
        {
            get => _appSettings.HintOpacity;
            set
            {
                _appSettings.HintOpacity = value;
                UpdatePreviewImages();
            }
        }

        public Font HintFont
        {
            get => _appSettings.HintFont;
            set
            {
                _appSettings.HintFont = value;
                UpdatePreviewImages();
            }
        }

        public Color HintTextColor
        {
            get => _appSettings.HintTextColor.GetValueOrDefault(Color.White);
            set
            {
                _appSettings.HintTextColor = value;
                UpdatePreviewImages();
            }
        }

        public HintColorSettingsForm()
        {
            InitializeComponent();

            _colorDialog = new ColorDialog();
            _imeColorItems = new BindingList<ImeColorItem>();

            _appSettings = AppSettings.Load();
            InitializeImeList();
            LoadFloatingHintSettings();
        }

        private void InitializeImeList()
        {
            // 先配置DataGridView的列结构
            SetupDataGridViewColumns();

            // 使用与主窗体相同的输入法列表
            // foreach (string lang in new[] { "中文", "英文" })
            // {
            //     var item = new ImeColorItem
            //     {
            //         ImeName = lang,
            //         Color = Color.Black // 默认颜色，稍后会从配置中加载
            //     };
            //     _imeColorItems.Add(item);
            // }
            foreach (var item in ImeColors)
            {
                _imeColorItems.Add(new ImeColorItem()
                {
                    LayoutName = item.LayoutName,
                    HintText = item.HintText,
                    Color = item.Color,
                });
            }

            // 设置数据源
            dgvHintColors.AutoGenerateColumns = false;
            dgvHintColors.DataSource = _imeColorItems;

            // 初始化预览图像
            InitializePreviewImages();
        }

        private void InitializePreviewImages()
        {
            int count = 0;
            foreach (var item in _imeColorItems)
            {
                PictureBox picPreview = new PictureBox()
                {
                    Location = new Point(20 + (count == 0 ? 0 : (picPreviews[count - 1].Width + 20)), 40),
                    // Location = new Point(20 + (int)(count * ((_appSettings.ImeColors[count].HintText.Length + 1) * 2 * HintFont.Size + 20)), 40),
                    BorderStyle = BorderStyle.None,
                    Font = HintFont
                };
                picPreview.Paint += (sender, e) =>
                {
                    DrawPreviewImage(picPreview, e.Graphics, item.HintText, item.Color);
                };
                picPreviews.Add(picPreview);
                this.Controls.Add(picPreview);
                count++;
            }

            // 创建中文预览图像
            // picChinesePreview = new PictureBox
            // {
            //     Location = new Point(20, 40),
            //     BorderStyle = BorderStyle.None,
            //     Font = HintFont
            // };
            // picChinesePreview.Paint += (sender, e) =>
            // {
            //     var chineseItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "中文");
            //     if (chineseItem != null)
            //     {
            //         DrawPreviewImage(picChinesePreview, e.Graphics, "中文", chineseItem.Color);
            //     }
            // };
            // this.Controls.Add(picChinesePreview);

            // // 创建英文预览图像
            // picEnglishPreview = new PictureBox
            // {
            //     Location = new Point(190, 40),
            //     BorderStyle = BorderStyle.None,
            //     Font = HintFont
            // };
            // picEnglishPreview.Paint += (sender, e) =>
            // {
            //     var englishItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "英文");
            //     if (englishItem != null)
            //     {
            //         DrawPreviewImage(picEnglishPreview, e.Graphics, "English", englishItem.Color);
            //     }
            // };
            // this.Controls.Add(picEnglishPreview);

            // 加载默认预览图像
            // UpdatePreviewImages();
        }

        private void DrawPreviewImage(PictureBox pictureBox, Graphics g, string text, Color hintColor)
        {

            var fontSize = g.MeasureString(text, HintFont).ToSize();
            int fontWidth = fontSize.Width;
            int fontHeight = fontSize.Height;

            var ellipeWidth = (int)HintFont.Size;
            var formWidth = fontWidth + ellipeWidth + 30;
            var formHeight = fontHeight + 8;

            // 动态设置预览图像尺寸
            pictureBox.Size = new Size(formWidth, formHeight);

            // 创建圆角效果
            var hrgn = WinApi.CreateRoundRectRgn(0, 0, pictureBox.Width, pictureBox.Height, 6, 6);
            pictureBox.Region = Region.FromHrgn(hrgn);
            WinApi.DeleteObject(hrgn);

            // 设置高质量渲染
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // 绘制半透明黑色背景
            // using (Brush bgBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            // System.Diagnostics.Debug.WriteLine(HintOpacity + "  " + HintBackColor.ToArgb());
            var bc = Color.FromArgb((int)(255 * HintOpacity),
               HintBackColor.R, HintBackColor.G, HintBackColor.B);
            using (Brush bgBrush = new SolidBrush(bc))
            {

                g.FillRectangle(bgBrush, 0, 0, pictureBox.Width, pictureBox.Height);
            }

            // 绘制颜色指示圆
            using (Brush colorBrush = new SolidBrush(hintColor))
            {
                g.FillEllipse(colorBrush, 10, ellipeWidth / 2 + 1, ellipeWidth, ellipeWidth);
            }

            // 绘制颜色指示圆边框
            using (Pen borderPen = new Pen(Color.White, 1))
            {
                g.DrawEllipse(borderPen, 10, ellipeWidth / 2 + 1, ellipeWidth, ellipeWidth);
            }

            // 绘制输入法名称
            using (Brush textBrush = new SolidBrush(TexColorSameHintColor ? pictureBox.ForeColor : HintTextColor))
            {
                g.DrawString(text, HintFont, textBrush, ellipeWidth + 20, 3);
            }

            // picEnglishPreview.Left = picChinesePreview.Width + 30;
            for (int i = 1; i < picPreviews.Count; i++)
            {
                picPreviews[i].Location = new Point(40 + (picPreviews[i - 1].Width), 40);
            }
            var w = picPreviews.Sum(pic => pic.Width) + 60;
            this.Width = w >= this.Width ? w : this.MinimumSize.Width;
            this.Height = 450 + (int)pictureBox.Height + 0;
            //this.panel1.Height = 400 + (int)FloatingHintFont.Size;
            //dgvHintColors.Top = pictureBox.Bottom;
        }

        private void UpdatePreviewImages()
        {
            for (int i = 0; i < picPreviews.Count; i++)
            {
                picPreviews[i].ForeColor = TexColorSameHintColor ? _imeColorItems[i].Color : HintTextColor;
                picPreviews[i].Invalidate(); // 触发重绘
            }

            // 更新中文预览图像
            // var chineseItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "中文");
            // if (chineseItem != null)
            // {
            //     // picChinesePreview.BackColor = FloatingHintBackColor;
            //     //     picChinesePreview.BackColor = Color.FromArgb(150 - (int)FloatingHintOpacity,
            //     //    FloatingHintBackColor.R, FloatingHintBackColor.G, FloatingHintBackColor.B);

            //     // picChinesePreview.Font = FloatingHintFont;
            //     picChinesePreview.ForeColor = TexColorSameHintColor ? chineseItem.Color : HintTextColor;
            //     picChinesePreview.Invalidate(); // 触发重绘
            // }

            // // 更新英文预览图像
            // var englishItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "英文");
            // if (englishItem != null)
            // {
            //     // picEnglishPreview.BackColor = FloatingHintBackColor;
            //     // picEnglishPreview.Font = FloatingHintFont;
            //     picEnglishPreview.ForeColor = TexColorSameHintColor ? englishItem.Color : HintTextColor;
            //     picEnglishPreview.Invalidate(); // 触发重绘
            // }

        }

        private void SetupDataGridViewColumns()
        {
            // 添加输入法列
            var imeColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(ImeColorItem.LayoutName),
                HeaderText = "键盘布局",
                ReadOnly = true,
                //AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 60,
                DataPropertyName = nameof(ImeColorItem.LayoutName)
            };
            dgvHintColors.Columns.Add(imeColumn);

            // 添加提示文本列
            var hintColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(ImeColorItem.HintText),
                HeaderText = "提示",
                ReadOnly = false,
                //AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 60,
                DataPropertyName = nameof(ImeColorItem.HintText)
            };
            dgvHintColors.Columns.Add(hintColumn);

            // 添加颜色预览列
            var colorPreviewColumn = new DataGridViewTextBoxColumn
            {
                Name = "ColorPreview",
                HeaderText = "颜色预览",
                Width = 100,
                ReadOnly = true,
                DataPropertyName = nameof(ImeColorItem.Color),

            };
            colorPreviewColumn.DefaultCellStyle.Padding = new Padding(10);
            dgvHintColors.Columns.Add(colorPreviewColumn);

            // 添加颜色名称列
            var colorNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "ColorName",
                HeaderText = "颜色名称",
                Width = 100,
                ReadOnly = true,
                DataPropertyName = nameof(ImeColorItem.Color)
            };
            dgvHintColors.Columns.Add(colorNameColumn);

            // 添加颜色选择按钮列
            var buttonColumn = new DataGridViewButtonColumn
            {
                Name = "SelectColor",
                HeaderText = "选择颜色",
                Text = "选择",
                UseColumnTextForButtonValue = true,
                Width = 80,
            };

            buttonColumn.DefaultCellStyle.Padding = new Padding(3, 2, 3, 2);
            dgvHintColors.Columns.Add(buttonColumn);

            // 订阅CellPainting事件以绘制颜色预览
            dgvHintColors.CellPainting += DgvHintColors_CellPainting;
            // 订阅CellFormatting事件以格式化颜色名称
            dgvHintColors.CellFormatting += DgvHintColors_CellFormatting;
        }

        private void DgvHintColors_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 绘制颜色预览单元格
            if (e.ColumnIndex >= 0 &&
                dgvHintColors.Columns[e.ColumnIndex].Name == "ColorPreview" &&
                e.RowIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);

                if (e.RowIndex < _imeColorItems.Count)
                {
                    var item = _imeColorItems[e.RowIndex];
                    var color = item.Color;

                    // 绘制颜色矩形
                    // var rect = new Rectangle(e.CellBounds.Left + 2, e.CellBounds.Top + 2,
                    //                         e.CellBounds.Width - 4, e.CellBounds.Height - 4);
                    var rect = new Rectangle(e.CellBounds.Left + e.CellBounds.Width / 2 - 10, e.CellBounds.Top + e.CellBounds.Height / 2 - 10,
                    20, 20);
                    using (var brush = new SolidBrush(color))
                    using (var borderPen = new Pen(Color.Black))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(borderPen, rect);
                    }
                }
                e.Handled = true;
            }
        }

        private void DgvHintColors_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 格式化颜色名称列
            if (e.ColumnIndex >= 0 &&
                dgvHintColors.Columns[e.ColumnIndex].Name == "ColorName" &&
                e.RowIndex >= 0 && e.RowIndex < _imeColorItems.Count)
            {
                var item = _imeColorItems[e.RowIndex];
                e.Value = item.Color.IsKnownColor ? item.Color.Name : $"#{item.Color.ToArgb() & 0xFFFFFF:X6}";
                e.FormattingApplied = true;
            }
        }

        // private void LoadColorSettings()
        // {
        //     foreach (var item in _imeColorItems)
        //     {
        //         if (ImeColors != null && ImeColors.TryGetValue(item.ImeName, out Color color))
        //         {
        //             item.Color = color;
        //         }
        //         else
        //         {
        //             // 设置默认颜色
        //             item.Color = GetDefaultColorForIme(item.ImeName);
        //         }
        //     }
        //     dgvHintColors.Refresh();
        //     // UpdatePreviewImages();
        // }

        private Color GetDefaultColorForIme(string imeName)
        {
            return imeName switch
            {
                "中文" => Color.Red,
                "英文" => Color.Lime,
                _ => Color.Black
            };
        }

        private void LoadFloatingHintSettings()
        {
            // 从应用程序设置加载悬浮提示窗的背景色、透明度、字体和文字颜色
            try
            {

                // 加载背景色

                HintBackColor = _appSettings.HintBackColor.GetValueOrDefault(Color.Black);


                // 加载透明度
                var opacityStr = _appSettings.HintOpacity.ToString();
                if (!string.IsNullOrEmpty(opacityStr) && double.TryParse(opacityStr, out double opacity))
                {
                    HintOpacity = opacity;
                }
                else
                {
                    HintOpacity = 0.6; // 默认透明度
                }

                // 加载字体
                HintFont = _appSettings.HintFont;
                // 加载文字颜色
                HintTextColor = _appSettings.HintTextColor.GetValueOrDefault(Color.White);
                //加载SameHintColor设置
                TexColorSameHintColor = _appSettings.TextColorSameHintColor;
            }
            catch
            {
                // 如果加载失败，使用默认值
                HintBackColor = Color.Black;
                HintOpacity = 0.6;
                HintFont = new Font("微软雅黑", 10, FontStyle.Bold);
                HintTextColor = Color.White;
            }
        }

        private void SaveFloatingHintSettings()
        {
            // 保存悬浮提示窗的背景色、透明度、字体和文字颜色到应用程序设置
            try
            {
                // var settings = AppSettings.Load();
                // settings.HintBackColor = HintBackColor;
                // settings.HintOpacity = HintOpacity;
                // settings.HintFont = HintFont;
                // settings.HintTextColor = HintTextColor;
                // settings.TextColorSameHintColor = TexColorSameHintColor; // 保存SameHintColor设置
                // settings.ImeColors = ImeColors;
                // settings.Save();
                // _appSettings.ImeColors = ImeColors;
                _appSettings.Save();
            }
            catch
            {
                // 忽略保存错误
            }
        }

        private void DgvHintColors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvHintColors.Columns["SelectColor"].Index && e.RowIndex >= 0)
            {
                var item = _imeColorItems[e.RowIndex];
                _colorDialog.Color = item.Color;

                if (_colorDialog.ShowDialog() == DialogResult.OK)
                {
                    item.Color = _colorDialog.Color;
                    // ImeColors[item.ImeName] = item.Color;
                    ImeColors.FirstOrDefault(x => x.LayoutName == item.LayoutName).Color = item.Color;
                    dgvHintColors.Refresh();
                    UpdatePreviewImages();
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // 保存颜色设置
            // ImeColors = new Dictionary<string, Color>();
            // foreach (var item in _imeColorItems)
            // {
            //     ImeColors[item.ImeName] = item.Color;
            // }

            // 保存悬浮提示窗设置
            SaveFloatingHintSettings();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void HintColorSettingsForm_Load(object sender, EventArgs e)
        {
            // 初始化悬浮提示窗设置控件
            InitializeFloatingHintControls();
        }

        private void InitializeFloatingHintControls()
        {
            // 设置背景色预览
            pnlBackColorPreview.BackColor = HintBackColor;
            lblBackColor.Text = convertColorName(HintBackColor);

            // 设置透明度滑块和标签
            trackOpacity.Value = (int)(HintOpacity * 100);
            lblOpacityValue.Text = $"{trackOpacity.Value}%";

            // 设置字体预览
            lblFontPreview.Text = $"{HintFont.Name}, {HintFont.Size:f2}pt,  {HintFont.Style}";
            //lblFontPreview.Font = FloatingHintFont;
            lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;

            // 设置文字颜色预览
            pnlTextColorPreview.BackColor = HintTextColor;
            lblTextColor.Text = convertColorName(HintTextColor);

            // 加载 SameHintColor 设置
            TexColorSameHintColor = TexColorSameHintColor;
            chkSameHintColor.Checked = TexColorSameHintColor;

            // 订阅 SameHintColor 复选框的事件
            chkSameHintColor.CheckedChanged += ChkSameHintColor_CheckedChanged;
        }

        private void ChkSameHintColor_CheckedChanged(object sender, EventArgs e)
        {
            TexColorSameHintColor = chkSameHintColor.Checked;
        }

        private void BtnBackColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = HintBackColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                HintBackColor = _colorDialog.Color;
                pnlBackColorPreview.BackColor = HintBackColor;
                lblBackColor.Text = convertColorName(HintBackColor);
            }
        }

        private string convertColorName(Color color)
        {
            return color.IsNamedColor ? color.Name : $"#{color.ToArgb() & 0xFFFFFF:X6}";
        }

        private void TrackOpacity_Scroll(object sender, EventArgs e)
        {
            HintOpacity = trackOpacity.Value / 100.0;
            lblOpacityValue.Text = $"{trackOpacity.Value}%";
        }

        private void BtnFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = HintFont;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                HintFont = fontDialog.Font;
                lblFontPreview.Text = $"{HintFont.Name}, {HintFont.Size:f2}pt, {HintFont.Style}";
                // lblFontPreview.Font = FloatingHintFont;
                lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;
            }
        }

        private void BtnTextColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = HintTextColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                HintTextColor = _colorDialog.Color;
                pnlTextColorPreview.BackColor = HintTextColor;
                lblTextColor.Text = convertColorName(HintTextColor);
            }
        }

        private void dgvHintColors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                _appSettings.ImeColors[e.RowIndex].HintText = _imeColorItems[e.RowIndex].HintText;
            }
        }
    }

    public class ImeColorItem : INotifyPropertyChanged
    {
        private string _imeName;
        private Color _color;
        private string _hintText;

        public string LayoutName
        {
            get => _imeName;
            set
            {
                _imeName = value;
                OnPropertyChanged("ImeName");
            }
        }

        public string HintText
        {
            get => _hintText;
            set
            {
                _hintText = value;
                OnPropertyChanged("HintText");
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}