using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SmartIme
{
    public partial class FloatingHintForm : Form
    {
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private void InitializeComponent()
        {

        }

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;
        const int LWA_ALPHA = 0x2;

        private readonly Color hintColor;
        private readonly string imeName;
        private readonly System.Windows.Forms.Timer closeTimer;

        public FloatingHintForm(Color color, string name)
        {
            hintColor = color;
            imeName = name;
            closeTimer = new System.Windows.Forms.Timer();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // 窗体设置
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(120, 60);
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // 添加绘制事件
            this.Paint += FloatingHintForm_Paint;

            // 设置自动关闭计时器（确保在UI线程上创建）
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    closeTimer.Interval = 1000;
                    closeTimer.Tick += CloseTimer_Tick;
                    closeTimer.Start();
                }));
            }
            else
            {
                closeTimer.Interval = 1000;
                closeTimer.Tick += CloseTimer_Tick;
                closeTimer.Start();
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            closeTimer.Dispose();
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer.Dispose();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 在句柄创建后设置窗口样式
            int initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, 0, 180, LWA_ALPHA); // 70% 透明度

            // 设置鼠标穿透
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        private void FloatingHintForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 绘制圆角矩形背景
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(220, Color.Black)))
            {
                g.FillRoundedRectangle(bgBrush, 0, 0, this.Width - 1, this.Height - 1, 10);
            }

            // 绘制颜色指示圆
            using (Brush colorBrush = new SolidBrush(hintColor))
            {
                g.FillEllipse(colorBrush, 10, 10, 20, 20);
            }

            // 绘制边框
            using (Pen borderPen = new Pen(Color.White, 1))
            {
                g.DrawEllipse(borderPen, 10, 10, 20, 20);
            }

            // 绘制输入法名称
            using (Font font = new Font("微软雅黑", 9, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                string displayName = imeName.Length > 8 ? imeName.Substring(0, 8) + "..." : imeName;
                g.DrawString(displayName, font, textBrush, 35, 15);
            }

            // 绘制颜色名称
            using (Font smallFont = new Font("微软雅黑", 7))
            using (Brush textBrush = new SolidBrush(Color.LightGray))
            {
                g.DrawString(hintColor.Name, smallFont, textBrush, 10, 35);
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            // 窗口失去焦点时保持置顶
            this.TopMost = true;
        }
    }

    // 扩展方法用于绘制圆角矩形
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
        {
            using (var path = CreateRoundedRectPath(x, y, width, height, radius))
            {
                graphics.FillPath(brush, path);
            }
        }
        
        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectPath(float x, float y, float width, float height, float radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            
            path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
            path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            
            return path;
        }
    }
}