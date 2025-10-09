using System.ComponentModel;

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
            set => _floatingHintBackColor = value;
        }

        public double FloatingHintOpacity
        {
            get => _floatingHintOpacity;
            set => _floatingHintOpacity = value;
        }

        public Font FloatingHintFont
        {
            get => _floatingHintFont;
            set => _floatingHintFont = value;
        }

        public Color FloatingHintTextColor
        {
            get => _floatingHintTextColor;
            set => _floatingHintTextColor = value;
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
                // 加载背景色
                var backColorStr = Properties.Settings.Default.FloatingHintBackColor;
                if (!string.IsNullOrEmpty(backColorStr))
                {
                    _floatingHintBackColor = ColorTranslator.FromHtml(backColorStr);
                }
                else
                {
                    _floatingHintBackColor = Color.Black; // 默认背景色
                }

                // 加载透明度
                var opacityStr = Properties.Settings.Default.FloatingHintOpacity;
                if (!string.IsNullOrEmpty(opacityStr) && double.TryParse(opacityStr, out double opacity))
                {
                    _floatingHintOpacity = opacity;
                }
                else
                {
                    _floatingHintOpacity = 0.6; // 默认透明度
                }

                // 加载字体
                var fontStr = Properties.Settings.Default.FloatingHintFont;
                if (!string.IsNullOrEmpty(fontStr))
                {
                    try
                    {
                        _floatingHintFont = (Font)new FontConverter().ConvertFromString(fontStr);
                    }
                    catch
                    {
                        _floatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold); // 默认字体
                    }
                }
                else
                {
                    _floatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold); // 默认字体
                }

                // 加载文字颜色
                var textColorStr = Properties.Settings.Default.FloatingHintTextColor;
                if (!string.IsNullOrEmpty(textColorStr))
                {
                    _floatingHintTextColor = ColorTranslator.FromHtml(textColorStr);
                }
                else
                {
                    _floatingHintTextColor = Color.White; // 默认文字颜色
                }
            }
            catch
            {
                // 如果加载失败，使用默认值
                _floatingHintBackColor = Color.Black;
                _floatingHintOpacity = 0.6;
                _floatingHintFont = new Font("微软雅黑", 10, FontStyle.Bold);
                _floatingHintTextColor = Color.White;
            }
        }

        private void SaveFloatingHintSettings()
        {
            // 保存悬浮提示窗的背景色、透明度、字体和文字颜色到应用程序设置
            try
            {
                Properties.Settings.Default.FloatingHintBackColor = ColorTranslator.ToHtml(_floatingHintBackColor);
                Properties.Settings.Default.FloatingHintOpacity = _floatingHintOpacity.ToString();
                Properties.Settings.Default.FloatingHintFont = new FontConverter().ConvertToString(_floatingHintFont);
                Properties.Settings.Default.FloatingHintTextColor = ColorTranslator.ToHtml(_floatingHintTextColor);
                Properties.Settings.Default.Save();
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
            pnlBackColorPreview.BackColor = _floatingHintBackColor;
            lblBackColor.Text = convertColorName(_floatingHintBackColor);

            // 设置透明度滑块和标签
            trackOpacity.Value = (int)(_floatingHintOpacity * 100);
            lblOpacityValue.Text = $"{trackOpacity.Value}%";

            // 设置字体预览
            // lblFontPreview.Text = $"{_floatingHintFont.Name}, {_floatingHintFont.Size}pt";
            lblFontPreview.Text = $"{_floatingHintFont.Name}";
            lblFontPreview.Font = _floatingHintFont;
            lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;

            // 设置文字颜色预览
            pnlTextColorPreview.BackColor = _floatingHintTextColor;
            lblTextColor.Text = convertColorName(_floatingHintTextColor);
        }

        private void BtnBackColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = _floatingHintBackColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                _floatingHintBackColor = _colorDialog.Color;
                pnlBackColorPreview.BackColor = _floatingHintBackColor;
                lblBackColor.Text = convertColorName(_floatingHintBackColor);
            }
        }

        private string convertColorName(Color color)
        {
            return color.IsNamedColor ? color.Name : $"#{color.ToArgb() & 0xFFFFFF:X6}";
        }

        private void TrackOpacity_Scroll(object sender, EventArgs e)
        {
            _floatingHintOpacity = trackOpacity.Value / 100.0;
            lblOpacityValue.Text = $"{trackOpacity.Value}%";
        }

        private void BtnFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = _floatingHintFont;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                _floatingHintFont = fontDialog.Font;
                // lblFontPreview.Text = $"{_floatingHintFont.Name}, {_floatingHintFont.Size}pt";
                lblFontPreview.Text = $"{_floatingHintFont.Name}";
                lblFontPreview.Font = _floatingHintFont;
                lblFontPreview.Top = label4.Top - (lblFontPreview.Height - label4.Height) / 2;
            }
        }

        private void BtnTextColor_Click(object sender, EventArgs e)
        {
            _colorDialog.Color = _floatingHintTextColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                _floatingHintTextColor = _colorDialog.Color;
                pnlTextColorPreview.BackColor = _floatingHintTextColor;
                lblTextColor.Text = convertColorName(_floatingHintTextColor);
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