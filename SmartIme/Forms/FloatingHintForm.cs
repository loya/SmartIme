using SmartIme.Utilities;
using System.Drawing.Drawing2D;

namespace SmartIme.Forms
{
    public partial class FloatingHintForm : Form
    {
        private const int _waitClose = 800; // 毫秒
        private readonly double _opacity; // 目标不透明度
        private readonly Color _hintColor; // 提示颜色
        private readonly string _imeName; // 输入法名称
        private readonly Color _backColor; // 背景颜色
        private readonly Font _font; // 字体
        private readonly Color _textColor; // 文字颜色
        private int formWidth = 100;
        private int _formHeight = 35;

        public FloatingHintForm(Color hintColor, string imeName, Color backColor, double opacity, Font font, Color textColor)
        {
            this._hintColor = hintColor;
            this._imeName = imeName;
            this._backColor = backColor;
            this._opacity = opacity;
            this._font = font;
            this._textColor = textColor;
            InitializeForm();
        }

        public FloatingHintForm(Color hintColor, string imeName, Color? backColor = null, double opacity = 0.6)
        {
            this._hintColor = hintColor;
            this._imeName = imeName;
            this._backColor = backColor ?? Color.Black;
            this._opacity = opacity;
            this._font = new Font("微软雅黑", 10, FontStyle.Bold);
            this._textColor = Color.White;
            InitializeForm();
        }
        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOPMOST = 0x00000008;
                // const int GWL_EXSTYLE = -20;

                CreateParams cp = base.CreateParams;
                // cp.ExStyle |= WS_EX_TOOLWINDOW; // 设置为工具窗口
                // cp.ExStyle |= WS_EX_NOACTIVATE; // 窗口不激活

                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOPMOST | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        private void InitializeForm()
        {
            // 窗体设置
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoSize = false;
            this.MinimumSize = Size.Empty;
            this.ShowInTaskbar = false;
            // this.TopMost = true;
            this.BackColor = _backColor; // 设置背景色为黑色
            this.Opacity = 0;
            this.Height = (int)(_font.Size + 8);
            this.Size = new Size(0, 0);
            this.ClientSize = new Size(0, 0);


            // 添加绘制事件
            this.Paint += FloatingHintForm_Paint;

            // 使用异步Task.Delay自动关闭窗口，避免使用Timer导致阻塞
            // _ = AutoCloseFormAsync();
        }

        private async Task AutoCloseFormAsync()
        {
            // 等待800毫秒后自动关闭窗口
            await Task.Delay(_waitClose);
            await FadeOutAsync();

            // 在UI线程中安全关闭窗口
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Close()));
            }
            else
            {
                this.Close();
            }
        }

        private void FloatingHintForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            string displayName = _imeName.Length > 8 ? _imeName.Substring(0, 6) + "..." : _imeName;
            var fontSize = g.MeasureString(displayName, _font).ToSize();
            int fontWidth = fontSize.Width;
            int fontHeight = fontSize.Height;


            _formHeight = fontHeight + 8;
            //圆宽度
            int ellipeWidth = (int)_font.Size;
            formWidth = fontWidth + ellipeWidth + 30;
            this.Size = new Size(formWidth, _formHeight);

            // 创建圆角效果（使用窗口尺寸）
            var hrgn = WinApi.CreateRoundRectRgn(0, 0, this.Width, this.Height, 6, 6);
            this.Region = Region.FromHrgn(hrgn);
            WinApi.DeleteObject(hrgn);

            // 设置高质量渲染
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // 绘制半透明黑色背景
            var bc = Color.FromArgb((int)(255 * _opacity),
               _backColor.R, _backColor.G, _backColor.B);
            using (Brush bgBrush = new SolidBrush(_backColor))// 40% 透明黑色
            // using (Brush bgBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0))) // 40% 透明黑色
            {
                // g.FillRectangle(bgBrush, -1, -1, this.Width, this.Height);
                // System.Diagnostics.Debug.WriteLine($"{this.Width},{this.ClientSize.Width}");

                // System.Diagnostics.Debug.WriteLine($"{this.Height},{this.ClientSize.Height}");
            }

            // 圆角边框由Region处理，不需要额外绘制

            // 绘制颜色指示圆
            using (Brush colorBrush = new SolidBrush(_hintColor))
            {
                g.FillEllipse(colorBrush, 10, (_formHeight - ellipeWidth) / 2, ellipeWidth, ellipeWidth);
            }

            // 绘制颜色指示圆边框
            using (Pen borderPen = new Pen(Color.White, 1))
            {
                g.DrawEllipse(borderPen, 10, (_formHeight - ellipeWidth) / 2, ellipeWidth, ellipeWidth);
            }

            // 绘制输入法名称
            using (Brush textBrush = new SolidBrush(_textColor))
            {
                if (displayName.Contains("(英)"))
                {
                    displayName = displayName.Replace("(英)", "");
                    g.DrawString(displayName, _font, textBrush, ellipeWidth + 20, 4);
                    using (Brush accentBrush = new SolidBrush(Color.SpringGreen)) // 亮绿色
                    {
                        g.DrawString("(A)", _font, accentBrush, ellipeWidth + g.MeasureString(displayName, _font).Width + 20, 4);
                    }
                }
                else
                    g.DrawString(displayName, _font, textBrush, ellipeWidth + 20, 4);
            }

            // formWidth = fontWidth + ellipeWidth + 30;
            // formHeight = fontHeight + 8;
            // this.Size = new Size(formWidth, formHeight);
            // // 创建圆角效果（使用窗口尺寸）
            // var hrgn = WinApi.CreateRoundRectRgn(0, 0, this.Width, this.Height, 6, 6);
            // this.Region = Region.FromHrgn(hrgn);
            // WinApi.DeleteObject(hrgn);

            // 绘制颜色名称
            //using (Font smallFont = new Font("微软雅黑", 7))
            //using (Brush textBrush = new SolidBrush(Color.LightGray))
            //{
            //    g.DrawString(hintColor.Name, smallFont, textBrush, 10, 35);
            //}
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            // 窗口失去焦点时保持置顶
            this.TopMost = true;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await FadeInAsync();
            await Task.Delay(_waitClose); // 停留时间
            await FadeOutAsync();
            this?.Close();
        }

        private async Task FadeInAsync()
        {
            //var start = DateTime.Now;
            //Debug.WriteLine(start.ToString("ffff"));
            for (double i = 0.1; i <= _opacity; i += 0.1)
            {
                this.Opacity = i;
                //System.Diagnostics.Debug.WriteLine("opacity in: " + i);
                await Task.Delay(10);
            }
            var end = DateTime.Now;
            //Debug.WriteLine(end.ToString("ffff"));
            //System.Diagnostics.Debug.WriteLine($"耗时：{end - start}");
        }

        private async Task FadeOutAsync()
        {
            //var start = DateTime.Now;
            for (double i = this.Opacity; i >= 0; i -= 0.05)
            {
                this.Opacity = i;
                //System.Diagnostics.Debug.WriteLine("opacity out3: " + i);
                await Task.Delay(10);
            }
            //System.Diagnostics.Debug.WriteLine($"耗时：{DateTime.Now - start}");

        }
    }

    // 扩展方法用于绘制圆角矩形
    //public static class GraphicsExtensions
    //{
    //    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
    //    {
    //        using (var path = CreateRoundedRectPath(x, y, width, height, radius))
    //        {
    //            graphics.FillPath(brush, path);
    //        }
    //    }

    //    private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectPath(float x, float y, float width, float height, float radius)
    //    {
    //        var path = new System.Drawing.Drawing2D.GraphicsPath();

    //        path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
    //        path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
    //        path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
    //        path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
    //        path.CloseFigure();

    //        return path;
    //    }
    //}

}