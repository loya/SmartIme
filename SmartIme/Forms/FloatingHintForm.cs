using SmartIme.Utilities;
using System.Drawing.Drawing2D;

namespace SmartIme.Forms
{
    public partial class FloatingHintForm : Form
    {
        private const double _opacity = 0.6;
        private const int _waitClose = 800; // 毫秒
        private readonly Color hintColor;
        private readonly string imeName;
        private int formWidth = 100;

        public FloatingHintForm(Color hintColor, string imeName)
        {
            this.hintColor = hintColor;
            this.imeName = imeName;
            InitializeForm();
        }

        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams
        {
            get
            {
                // const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOPMOST = 0x00000008;
                // const int GWL_EXSTYLE = -20;

                CreateParams cp = base.CreateParams;
                // cp.ExStyle |= WS_EX_TOOLWINDOW; // 设置为工具窗口
                // cp.ExStyle |= WS_EX_NOACTIVATE; // 窗口不激活

                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOPMOST;
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
            this.BackColor = Color.Black; // 设置背景色为黑色
            this.Opacity = _opacity;

            // 添加绘制事件
            this.Paint += FloatingHintForm_Paint;

            // 使用异步Task.Delay自动关闭窗口，避免使用Timer导致阻塞
            _ = AutoCloseFormAsync();
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

            // 设置高质量渲染
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // 绘制半透明黑色背景
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0))) // 40% 透明黑色
            {
                g.FillRectangle(bgBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }

            // 圆角边框由Region处理，不需要额外绘制

            // 绘制颜色指示圆
            using (Brush colorBrush = new SolidBrush(hintColor))
            {
                g.FillEllipse(colorBrush, 10, 10, 14, 14);
            }

            // 绘制边框
            using (Pen borderPen = new Pen(Color.White, 1))
            {
                g.DrawEllipse(borderPen, 10, 10, 14, 14);
            }

            // 绘制输入法名称
            using (Font font = new Font("微软雅黑", 10, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                string displayName = imeName.Length > 8 ? imeName.Substring(0, 6) + "..." : imeName;
                if (displayName.Contains("(英)"))
                {
                    displayName = displayName.Replace("(英)", "");
                    g.DrawString(displayName, font, textBrush, 28, 8);
                    using (Brush accentBrush = new SolidBrush(Color.SpringGreen)) // 亮绿色
                    {
                        g.DrawString("(A)", font, accentBrush, 28 + g.MeasureString(displayName, font).Width + 1, 7);
                    }
                }
                g.DrawString(displayName, font, textBrush, 28, 8);
            }

            formWidth = (int)g.MeasureString(imeName, new Font("微软雅黑", 10, FontStyle.Bold)).Width + 40;
            this.Size = new Size(formWidth, 35);
            // 创建圆角效果（使用窗口尺寸）
            var hrgn = WinApi.CreateRoundRectRgn(0, 0, this.Width, this.Height, 6, 6);
            this.Region = Region.FromHrgn(hrgn);
            WinApi.DeleteObject(hrgn);

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
            //this.TopMost = true;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await FadeInAsync();
            //await Task.Delay(1000); // 停留时间
            //await FadeOutAsync();
            //this.Close();
        }

        private async Task FadeInAsync()
        {
            for (double i = 0; i <= _opacity; i += 0.1)
            {
                this.Opacity = i;
                await Task.Delay(20);
            }
        }

        private async Task FadeOutAsync()
        {
            for (double i = this.Opacity; i >= 0; i -= 0.1)
            {
                this.Opacity = i;
                await Task.Delay(20);
            }
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