using SmartIme.Utilities;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SmartIme.Forms
{
    public partial class HintColorSettingsForm : Form
    {
        private Dictionary<string, Color> _imeColors;
        private ColorDialog _colorDialog;
        private BindingList<ImeColorItem> _imeColorItems;
        private Color _floatingHintBackColor;
        private double _floatingHintOpacity;
        private Font _floatingHintFont;
        private Color _floatingHintTextColor;
        private bool _sameHintColor;
        private PictureBox picChinesePreview;
        private PictureBox picEnglishPreview;

        public bool SameHintColor
        {
            get => _sameHintColor;
            set
            {
                _sameHintColor = value;
                if (chkSameHintColor != null)
                    chkSameHintColor.Checked = value;
                UpdatePreviewImages();
            }
        }

        public Dictionary<string, Color> ImeColors
        {
            get => _imeColors;
            set
            {
                _imeColors = value;
                // 如果_imeColorItems已经初始化，立即加载颜色设置
                if (_imeColorItems != null && _imeColorItems.Count > 0)
                {
                    LoadColorSettings();
                }
            }
        }

        public Color FloatingHintBackColor
        {
            get => _floatingHintBackColor;
            set
            {
                _floatingHintBackColor = value;
                UpdatePreviewImages();
            }
        }

        public double FloatingHintOpacity
        {
            get => _floatingHintOpacity;
            set
            {
                _floatingHintOpacity = value;
                UpdatePreviewImages();
            }
        }

        public Font FloatingHintFont
        {
            get => _floatingHintFont;
            set
            {
                _floatingHintFont = value;
                UpdatePreviewImages();
            }
        }

        public Color FloatingHintTextColor
        {
            get => _floatingHintTextColor;
            set
            {
                _floatingHintTextColor = value;
                UpdatePreviewImages();
            }
        }

        public HintColorSettingsForm()
        {
            InitializeComponent();


            _colorDialog = new ColorDialog();
            _imeColorItems = new BindingList<ImeColorItem>();
            InitializeImeList();
            LoadFloatingHintSettings();
        }

        private void InitializeImeList()
        {
            // 先配置DataGridView的列结构
            SetupDataGridViewColumns();

            // 使用与主窗体相同的输入法列表
            foreach (string lang in new[] { "中文", "英文" })
            {
                var item = new ImeColorItem
                {
                    ImeName = lang,
                    Color = Color.Black // 默认颜色，稍后会从配置中加载
                };
                _imeColorItems.Add(item);
            }

            // 设置数据源
            dgvHintColors.AutoGenerateColumns = false;
            dgvHintColors.DataSource = _imeColorItems;

            // 初始化预览图像
            InitializePreviewImages();
        }

        private void InitializePreviewImages()
        {
            // 创建中文预览图像
            picChinesePreview = new PictureBox
            {
                Location = new Point(20, 40),
                BorderStyle = BorderStyle.None,
                BackColor = FloatingHintBackColor,
                Font = FloatingHintFont
            };
            picChinesePreview.Paint += (sender, e) =>
            {
                var chineseItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "中文");
                if (chineseItem != null)
                {
                    DrawPreviewImage(picChinesePreview, e.Graphics, "中文", chineseItem.Color);
                }
            };
            this.Controls.Add(picChinesePreview);

            // 创建英文预览图像
            picEnglishPreview = new PictureBox
            {
                Location = new Point(190, 40),
                BorderStyle = BorderStyle.None,
                BackColor = FloatingHintBackColor,
                Font = FloatingHintFont
            };
            picEnglishPreview.Paint += (sender, e) =>
            {
                var englishItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "英文");
                if (englishItem != null)
                {
                    DrawPreviewImage(picEnglishPreview, e.Graphics, "English", englishItem.Color);
                }
            };
            this.Controls.Add(picEnglishPreview);

            // 加载默认预览图像
            UpdatePreviewImages();
        }

        private void DrawPreviewImage(PictureBox pictureBox, Graphics g, string text, Color hintColor)
        {

            var fontSize = g.MeasureString(text, FloatingHintFont).ToSize();
            int fontWidth = fontSize.Width;
            int fontHeight = fontSize.Height;

            var ellipeWidth = (int)FloatingHintFont.Size;
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
            System.Diagnostics.Debug.WriteLine(FloatingHintOpacity);
            using (Brush bgBrush = new SolidBrush(Color.FromArgb((int)(255 * FloatingHintOpacity),
               FloatingHintBackColor.R, FloatingHintBackColor.G, FloatingHintBackColor.B)))
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
            using (Brush textBrush = new SolidBrush(SameHintColor ? pictureBox.ForeColor : FloatingHintTextColor))
            {
                g.DrawString(text, FloatingHintFont, textBrush, ellipeWidth + 20, 3);
            }

            picEnglishPreview.Left = picChinesePreview.Width + 30;
            var w = picChinesePreview.Width + picEnglishPreview.Width + 60;
            this.Width = w >= this.Width ? w : this.MinimumSize.Width;
            this.Height = 450 + (int)pictureBox.Height + 0;
            //this.panel1.Height = 400 + (int)FloatingHintFont.Size;
            //dgvHintColors.Top = pictureBox.Bottom;
        }

        private void UpdatePreviewImages()
        {
            // 更新中文预览图像
            var chineseItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "中文");
            if (chineseItem != null)
            {
                // picChinesePreview.BackColor = FloatingHintBackColor;
                //     picChinesePreview.BackColor = Color.FromArgb(150 - (int)FloatingHintOpacity,
                //    FloatingHintBackColor.R, FloatingHintBackColor.G, FloatingHintBackColor.B);

                // picChinesePreview.Font = FloatingHintFont;
                picChinesePreview.ForeColor = SameHintColor ? chineseItem.Color : FloatingHintTextColor;
                picChinesePreview.Invalidate(); // 触发重绘
            }

            // 更新英文预览图像
            var englishItem = _imeColorItems.FirstOrDefault(item => item.ImeName == "英文");
            if (englishItem != null)
            {
                // picEnglishPreview.BackColor = FloatingHintBackColor;
                // picEnglishPreview.Font = FloatingHintFont;
                picEnglishPreview.ForeColor = SameHintColor ? englishItem.Color : FloatingHintTextColor;
                picEnglishPreview.Invalidate(); // 触发重绘
            }

        }

        private void SetupDataGridViewColumns()
        {
            // 添加输入法列
            var imeColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(ImeColorItem.ImeName),
                HeaderText = "输入法",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 60,
                DataPropertyName = nameof(ImeColorItem.ImeName)
            };
            dgvHintColors.Columns.Add(imeColumn);

            // 添加颜色预览列
            var colorPreviewColumn = new DataGridViewTextBoxColumn
            {
                Name = "ColorPreview",
                HeaderText = "颜色预览",
                Width = 100,
                ReadOnly = true,
                DataPropertyName = nameof(ImeColorItem.Color)
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

        private void LoadColorSettings()
        {
            foreach (var item in _imeColorItems)
            {
                if (_imeColors != null && _imeColors.TryGetValue(item.ImeName, out Color color))
                {
                    item.Color = color;
                }
                else
                {
                    // 设置默认颜色
                    item.Color = GetDefaultColorForIme(item.ImeName);
                }
            }
            dgvHintColors.Refresh();
            // UpdatePreviewImages();
        }

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
                var settings = AppSettings.Load();
                // 加载背景色
                var backColorStr = settings.HintBackColor;
                if (!string.IsNullOrEmpty(backColorStr))
                {
                    FloatingHintBackColor = ColorTranslator.FromHtml(backColorStr);
                }
                else
                {
                    FloatingHintBackColor = Color.Black; // 默认背景色
                }

                // 加载透明度
                var opacityStr = settings.HintOpacity.ToString();
                if (!string.IsNullOrEmpty(opacityStr) && double.TryParse(opacityStr, out double opacity))
                {
                    FloatingHintOpacity = opacity;
                }
                else
                {
                    FloatingHintOpacity = 0.6; // 默认透明度
                }

                // 加载字体
                var fontStr = settings.HintFont;
                if (!string.IsNullOrEmpty(fontStr))
                {
                    try
                    {
                        FloatingHintFont = (Font)new FontConverter().ConvertFromString(fontStr);
                    }
                    catch
                    {
                        FloatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold); // 默认字体
                    }
                }
                else
                {
                    FloatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold); // 默认字体
                }

                // 加载文字颜色
                var textColorStr = settings.HintTextColor;
                if (!string.IsNullOrEmpty(textColorStr))
                {
                    FloatingHintTextColor = ColorTranslator.FromHtml(textColorStr);
                }
                else
                {
                    FloatingHintTextColor = Color.White; // 默认文字颜色
                }

                //加载SameHintColor设置
                SameHintColor = settings.SameHintColor;
            }
            catch
            {
                // 如果加载失败，使用默认值
                FloatingHintBackColor = Color.Black;
                FloatingHintOpacity = 0.6;
                FloatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold);
                FloatingHintTextColor = Color.White;
            }
        }

        private void SaveFloatingHintSettings()
        {
            // 保存悬浮提示窗的背景色、透明度、字体和文字颜色到应用程序设置
            try
            {
                var settings = AppSettings.Load();
                settings.HintBackColor = ColorTranslator.ToHtml(FloatingHintBackColor);
                settings.HintOpacity = FloatingHintOpacity;
                settings.HintFont = new FontConverter().ConvertToString(FloatingHintFont);
                settings.HintTextColor = ColorTranslator.ToHtml(FloatingHintTextColor);
                settings.SameHintColor = SameHintColor; // 保存SameHintColor设置
                settings.Save();
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
                    dgvHintColors.Refresh();
                    UpdatePreviewImages();
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // 保存颜色设置
            _imeColors = new Dictionary<string, Color>();
            foreach (var item in _imeColorItems)
            {
                _imeColors[item.ImeName] = item.Color;
            }

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
            pnlBackColorPreview.BackColor = FloatingHintBackColor;
            lblBackColor.Text = convertColorName(FloatingHintBackColor);

            // 设置透明度滑块和标签
            trackOpacity.Value = (int)(FloatingHintOpacity * 100);
            lblOpacityValue.Text = $"{trackOpacity.Value}%";

            // 设置字体预览
            lblFontPreview.Text = $"{FloatingHintFont.Name}, {FloatingHintFont.Size}pt";
            //lblFontPreview.Font = FloatingHintFont;
            lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;

            // 设置文字颜色预览
            pnlTextColorPreview.BackColor = FloatingHintTextColor;
            lblTextColor.Text = convertColorName(FloatingHintTextColor);

            // 加载 SameHintColor 设置
            SameHintColor = SameHintColor;
            chkSameHintColor.Checked = SameHintColor;

            // 订阅 SameHintColor 复选框的事件
            chkSameHintColor.CheckedChanged += ChkSameHintColor_CheckedChanged;
        }

        private void ChkSameHintColor_CheckedChanged(object sender, EventArgs e)
        {
            SameHintColor = chkSameHintColor.Checked;
        }

        private void BtnBackColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = FloatingHintBackColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                FloatingHintBackColor = _colorDialog.Color;
                pnlBackColorPreview.BackColor = FloatingHintBackColor;
                lblBackColor.Text = convertColorName(FloatingHintBackColor);
            }
        }

        private string convertColorName(Color color)
        {
            return color.IsNamedColor ? color.Name : $"#{color.ToArgb() & 0xFFFFFF:X6}";
        }

        private void TrackOpacity_Scroll(object sender, EventArgs e)
        {
            FloatingHintOpacity = trackOpacity.Value / 100.0;
            lblOpacityValue.Text = $"{trackOpacity.Value}%";
        }

        private void BtnFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = FloatingHintFont;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                FloatingHintFont = fontDialog.Font;
                lblFontPreview.Text = $"{FloatingHintFont.Name}, {FloatingHintFont.Size}pt";
                // lblFontPreview.Font = FloatingHintFont;
                lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;
            }
        }

        private void BtnTextColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = FloatingHintTextColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                FloatingHintTextColor = _colorDialog.Color;
                pnlTextColorPreview.BackColor = FloatingHintTextColor;
                lblTextColor.Text = convertColorName(FloatingHintTextColor);
            }
        }
    }

    public class ImeColorItem : INotifyPropertyChanged
    {
        private string _imeName;
        private Color _color;

        public string ImeName
        {
            get => _imeName;
            set
            {
                _imeName = value;
                OnPropertyChanged("ImeName");
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